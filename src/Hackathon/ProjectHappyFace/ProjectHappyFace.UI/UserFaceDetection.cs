using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ProjectHappyFace.UI
{
    public class UserFaceDetection
    {
        private string tempStoragePath;

        private async Task DetectUserImage(string filePath , string CollectionName)
        {
            string statusMessage = string.Empty;

            // Give me the file full path

            var fileName = filePath;
                //

            if (fileName != null)
            {
                var SelectedFile = fileName;

                // Clear last detection result
                ResultCollection.Clear();
                DetectedFaces.Clear();

                FileInfo file = new FileInfo(SelectedFile);
                var sizeInBytes = file.Length;

                Bitmap img = new Bitmap(SelectedFile);

                var imageInfo = new Tuple<int, int>(img.Height, img.Width);


                // Call detection REST API
                using (var fileStream = File.OpenRead(SelectedFile))
                {
                    try
                    {
                        var faceServiceClient = CognitiveServiceFramework.CreateFaceServiceClient();
                        Microsoft.ProjectOxford.Face.Contract.Face[] faces;

                        if (CollectionName == "Result")
                        {
                            faces = await faceServiceClient.DetectAsync(fileStream, false, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses, FaceAttributeType.HeadPose, FaceAttributeType.FacialHair });
                        }
                        else
                        {
                            faces = await faceServiceClient.DetectAsync(fileStream);
                        }

                        string DetectedResultsInText = string.Format("{0} face(s) has been detected", faces.Length);

                        //foreach (var face in faces)
                        //{
                        //    DetectedFaces.Add(new Face()
                        //    {
                        //        ImagePath = SelectedFile,
                        //        Left = face.FaceRectangle.Left,
                        //        Top = face.FaceRectangle.Top,
                        //        Width = face.FaceRectangle.Width,
                        //        Height = face.FaceRectangle.Height,
                        //        FaceId = face.FaceId.ToString(),
                        //        Gender = face.FaceAttributes.Gender,
                        //        Age = string.Format("{0:#} years old", face.FaceAttributes.Age),
                        //        IsSmiling = face.FaceAttributes.Smile > 0.0 ? "Smile" : "Not Smile",
                        //        Glasses = face.FaceAttributes.Glasses.ToString(),
                        //        FacialHair = string.Format("Facial Hair: {0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No"),
                        //        HeadPose = string.Format("Pitch: {0}, Roll: {1}, Yaw: {2}", Math.Round(face.FaceAttributes.HeadPose.Pitch, 2), Math.Round(face.FaceAttributes.HeadPose.Roll, 2), Math.Round(face.FaceAttributes.HeadPose.Yaw, 2))
                        //    });
                        //}


                        // Convert detection result into UI binding object for rendering
                        foreach (var face in UIHelper.CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                        {
                            if (CollectionName == "Result")
                            {
                                ResultCollection.Add(face);
                            }
                            else if (CollectionName == "UserProvided")
                            {
                                LeftResultCollection.Add(face);
                            }
                            else
                            {
                                RightResultCollection.Add(face);
                            }
                            
                        }

                    }
                    catch (FaceAPIException ex)
                    {
                        statusMessage = "Error : " + ex.ErrorMessage;
                        GC.Collect();
                    }
                    finally
                    {
                        GC.Collect();
                    }
                }
            }
        }

        public async Task VerifyUserImageForRegister( string path)
        {
            await DetectUserImage(path , "Result");
        }

        public Face GetUserData()
        {
            var face = new Face();
            if (ResultCollection.Count > 0)
            {
                foreach (var item in ResultCollection)
                {
                    face.Age = item.Age;
                    face.Gender = item.Gender;
                    face.FaceId = item.FaceId;
                    break;
                }

                return face;
            }
            else
            {
                return face;
            }
        }


        public void SaveUserData(UserData data)
        {
            var dataBaseCall = new DBCall();

            var status = dataBaseCall.StoreUSerDataInDatabase(data);

        }

        public async Task<string> CompareAndAuthenticateData(string UserwebcamPath)
        {
            string data = string.Empty;

            //Bring the User Image provided from web cam and stored in a temp directory or the left collection data

            LeftResultCollection.Clear();

            await DetectUserImage(UserwebcamPath, "UserProvided");


            //Now call the db to bring back the byte data

            

            //Now save the byte data in an image format in a place.
           var dbImagePath = SaveTempImageTakenFromDatabase(data);

            RightResultCollection.Clear();

            await DetectUserImage(dbImagePath, "Right");

            await Face2FaceVerification();

            if (!string.IsNullOrEmpty(FaceVerifyResult))
            {
                return FaceVerifyResult;
            }
            else
            {
                return string.Empty;
            }

        }

        private string SaveTempImageTakenFromDatabase(string data)
        {
            string absPath;

            data = data.Substring(22);
            byte[] bytes = Convert.FromBase64String(data);
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);

                absPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data");
                image.Save(absPath + "\\abc.bmp");
            }

            

            return (absPath + "\\abc.bmp");
        }


        public async Task<bool> CompareAndAuthenticate(byte[] source, byte[] target)
        {
            var faceServiceClient = CognitiveServiceFramework.CreateFaceServiceClient();
            Microsoft.ProjectOxford.Face.Contract.Face[] srcFaces;
            Microsoft.ProjectOxford.Face.Contract.Face[] destFaces;
            Bitmap imgSrc = null; 
            Bitmap imgDest = null;
            using (var srcFileStream = new MemoryStream(source))
            {
                
                srcFaces = await faceServiceClient.DetectAsync(srcFileStream);
                
            }
            using (var descFileStream = new MemoryStream(target))
            {
                
                destFaces = await faceServiceClient.DetectAsync(descFileStream);
                
            }

            using (var srcFileStream = new MemoryStream(source))
            {
                imgSrc = new Bitmap(srcFileStream);
            }
            using (var descFileStream = new MemoryStream(target))
            {
                imgDest = new Bitmap(descFileStream);
            }

            var imageInfoSrc = new Tuple<int, int>(imgSrc.Height, imgSrc.Width);
            var imageInfoDest= new Tuple<int, int>(imgDest.Height, imgDest.Width);

            if (srcFaces != null && srcFaces.Count() > 0 && destFaces != null && destFaces.Count() > 0)
            {
                foreach (var face in UIHelper.CalculateFaceRectangleForRendering(srcFaces, MaxImageSize,imageInfoSrc))
                {
                    LeftResultCollection.Add(face);

                }
                foreach (var face in UIHelper.CalculateFaceRectangleForRendering(destFaces, MaxImageSize, imageInfoDest))
                {
                    RightResultCollection.Add(face);
                }

                return await Face2FaceVerification();

            }



            return false;

        }



        private async Task<bool> Face2FaceVerification()
        {
            // Call face to face verification, verify REST API supports one face to one face verification only
            // Here, we handle single face image only
            if (LeftResultCollection.Count == 1 && RightResultCollection.Count == 1)
            {
                FaceVerifyResult = "Verifying...";
                var faceId1 = LeftResultCollection[0].FaceId;
                var faceId2 = RightResultCollection[0].FaceId;


                // Call verify REST API with two face id
                try
                {
                    var faceServiceClient = CognitiveServiceFramework.CreateFaceServiceClient();
                    var res = await faceServiceClient.VerifyAsync(Guid.Parse(faceId1), Guid.Parse(faceId2));

                    // Verification result contains IsIdentical (true or false) and Confidence (in range 0.0 ~ 1.0),
                    // here we update verify result on UI by FaceVerifyResult binding
                    FaceVerifyResult = string.Format("Confidence = {0:0.00}, {1}", res.Confidence, res.IsIdentical ? "two faces belong to same person" : "two faces not belong to same person");

                    return res.IsIdentical;
                }
                catch (FaceAPIException ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
               // MessageBox.Show("Verification accepts two faces as input, please pick images with only one detectable face in it.", "Warning", MessageBoxButton.OK);
            }
            GC.Collect();
        }

    











        #region Properties
        private List<Face> _detectedFaces = new List<Face>();
        
        private ObservableCollection<Face> _resultCollection = new ObservableCollection<Face>();
        private ObservableCollection<Face> _leftResultCollection = new ObservableCollection<Face>();
        private ObservableCollection<Face> _rightResultCollection = new ObservableCollection<Face>();

        public string FaceVerifyResult { get; set; }

        public List<Face> DetectedFaces
        {
            get
            {
                return _detectedFaces;
            }
        }

        public int MaxImageSize
        {
            get
            {
                return 300;
            }
        }


        /// <summary>
        /// Gets face detection results
        /// </summary>
        public ObservableCollection<Face> ResultCollection
        {
            get
            {
                return _resultCollection;
            }
        }

        public ObservableCollection<Face> LeftResultCollection
        {
            get
            {
                return _leftResultCollection;
            }
        }

        public ObservableCollection<Face> RightResultCollection
        {
            get
            {
                return _rightResultCollection;
            }
        }

        #endregion
    }
}