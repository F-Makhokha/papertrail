using System;
using System.IO;
using System.Text;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace PaperTrail
{
    public class VisionProvider
    {
        /// <summary>
        /// The vision service client.
        /// </summary>
        private readonly IVisionServiceClient _visionClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisionProvider"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        public VisionProvider(string subscriptionKey)
        {
            this._visionClient = new VisionServiceClient(subscriptionKey);
        }

        /// <summary>
        /// Recognize text from given image.
        /// </summary>
        /// <param name="imagePathOrUrl">The image path or url.</param>
        /// <param name="detectOrientation">if set to <c>true</c> [detect orientation].</param>
        /// <param name="languageCode">The language code.</param>
        public void RecognizeText(string imagePathOrUrl, bool detectOrientation = true, string languageCode = LanguageCodes.AutoDetect)
        {
            this.ShowInfo("Recognizing");
            OcrResults ocrResult = null;
            string resultStr = string.Empty;

            try
            {
                if (File.Exists(imagePathOrUrl))
                {
                    using (FileStream stream = File.Open(imagePathOrUrl, FileMode.Open))
                    {
                        ocrResult = this._visionClient.RecognizeTextAsync(stream, languageCode, detectOrientation).Result;
                    }
                }
                else if (Uri.IsWellFormedUriString(imagePathOrUrl, UriKind.Absolute))
                {
                    ocrResult = this._visionClient.RecognizeTextAsync(imagePathOrUrl, languageCode, detectOrientation).Result;
                }
                else
                {
                    this.ShowError("Invalid image path or Url");
                }
            }
            catch (ClientException e)
            {
                if (e.Error != null)
                {
                    this.ShowError(e.Error.Message);
                }
                else
                {
                    this.ShowError(e.Message);
                }

                return;
            }
            catch (Exception)
            {
                this.ShowError("Some error happened.");
                return;
            }

            this.ShowRetrieveText(ocrResult);
        }

        /// <summary>
        /// Retrieve text from the given OCR results object.
        /// </summary>
        /// <param name="results">The OCR results.</param>
        /// <returns>Return the text.</returns>
        private void ShowRetrieveText(OcrResults results)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (results != null && results.Regions != null)
            {
                stringBuilder.Append("Text: ");
                stringBuilder.AppendLine();
                foreach (var item in results.Regions)
                {
                    foreach (var line in item.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            stringBuilder.Append(word.Text);
                            stringBuilder.Append(" ");
                        }

                        stringBuilder.AppendLine();
                    }

                    stringBuilder.AppendLine();
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(stringBuilder.ToString());
            Console.ResetColor();
        }

        /// <summary>
        /// Show the working item.
        /// </summary>
        /// <param name="workStr">The work item's string.</param>
        private void ShowInfo(string workStr)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format("{0}......", workStr));
            Console.ResetColor();
        }

        /// <summary>
        /// Show error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        private void ShowError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
        }

        /// <summary>
        /// Show result message.
        /// </summary>
        /// <param name="resultMessage">The result message.</param>
        private void ShowResult(string resultMessage)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(resultMessage);
            Console.ResetColor();
        }
    }
}
