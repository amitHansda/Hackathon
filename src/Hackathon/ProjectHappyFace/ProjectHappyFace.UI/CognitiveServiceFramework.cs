using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectHappyFace.UI
{
    public static class CognitiveServiceFramework
    {
        public static FaceServiceClient CreateFaceServiceClient()
        {
            string subscriptionKey = @"012ddb775b0b4285a89e6643cf947757";

            return new FaceServiceClient(subscriptionKey);
        }
    }
}