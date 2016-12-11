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

        public async Task DetectUserImage()
        {
            string statusMessage = string.Empty;

            // Give me the file full path

            //var fileName = @"C:\Users\nirotpalm626\Desktop\Hackathon\Cognitive-Face-Windows-master\Cognitive-Face-Windows-master\Data\PersonGroup\Family1-Dad\Family1-Dad1.jpg";
            var fileName = @"C:\Users\amith090\Downloads\Cognitive-Face-Windows-master\Cognitive-Face-Windows-master\Data\detection2.jpg";
            if (fileName != null)
            {
                var SelectedFile = fileName;

                // Clear last detection result
                ResultCollection.Clear();
                DetectedFaces.Clear();

                FileInfo file = new FileInfo(SelectedFile);
                var sizeInBytes = file.Length;

                Bitmap img = new Bitmap(SelectedFile);

                var imageHeight = img.Height;
                var imageWidth = img.Width;

                var imageInfo = new Tuple<int, int>(img.Height, img.Width);


                // Call detection REST API
                using (var fileStream = File.OpenRead(SelectedFile))
                {
                    try
                    {
                        var faceServiceClient = CognitiveServiceFramework.CreateFaceServiceClient();

                        Microsoft.ProjectOxford.Face.Contract.Face[] faces = await faceServiceClient.DetectAsync(fileStream, false, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses, FaceAttributeType.HeadPose, FaceAttributeType.FacialHair });

                        string DetectedResultsInText = string.Format("{0} face(s) has been detected", faces.Length);

                        foreach (var face in faces)
                        {
                            DetectedFaces.Add(new Face()
                            {
                                ImagePath = SelectedFile,
                                Left = face.FaceRectangle.Left,
                                Top = face.FaceRectangle.Top,
                                Width = face.FaceRectangle.Width,
                                Height = face.FaceRectangle.Height,
                                FaceId = face.FaceId.ToString(),
                                Gender = face.FaceAttributes.Gender,
                                Age = string.Format("{0:#} years old", face.FaceAttributes.Age),
                                IsSmiling = face.FaceAttributes.Smile > 0.0 ? "Smile" : "Not Smile",
                                Glasses = face.FaceAttributes.Glasses.ToString(),
                                FacialHair = string.Format("Facial Hair: {0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No"),
                                HeadPose = string.Format("Pitch: {0}, Roll: {1}, Yaw: {2}", Math.Round(face.FaceAttributes.HeadPose.Pitch, 2), Math.Round(face.FaceAttributes.HeadPose.Roll, 2), Math.Round(face.FaceAttributes.HeadPose.Yaw, 2))
                            });
                        }


                        // Convert detection result into UI binding object for rendering
                        foreach (var face in UIHelper.CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                        {
                            ResultCollection.Add(face);
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
                   // return statusMessage;
                   // return DetectedFaces;
                }
            }
        }

        public async Task VerifyUserImageForRegister()
        {
            //Task<string> t = null;

          

            //DetectUserImage();

            //var task = new Task(DetectUserImage);
            //task.Start();
            //task.Wait();

            await DetectUserImage();

            //DetectUserImage().Wait();


            //Task Auth0TokenTask = Task.Factory.StartNew(() =>
            //{
            //    var a = DetectUserImage();
            //});

            //Task.WaitAll(Auth0TokenTask);

           // var a = DetectUserImage();

            //var task = DetectUserImage();
            //task.Start();
            //task.Wait();


           // return new Face();
            
        }

        public Face GetUserData()
        {
            var face = new Face();
            if (DetectedFaces.Count > 0)
            {
                foreach (var item in DetectedFaces)
                {
                    face.Age = item.Age;
                    face.Gender = item.Gender;
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



        #region Properties
        private List<Face> _detectedFaces = new List<Face>();
        
        private ObservableCollection<Face> _resultCollection = new ObservableCollection<Face>();

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

        #endregion
    }
}