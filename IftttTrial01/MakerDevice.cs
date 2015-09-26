using System;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using GrFamily.MainBoard;
using GrFamily.ExternalBoard;
using GrFamily.Utility;

namespace IftttTrial01
{
    public class MakerDevice
    {
        private string _iftttAddress = "http://maker.ifttt.com/trigger/" +
            Resources.GetString(Resources.StringResources.MakerEventName) +
            "/with/key/" +
            Resources.GetString(Resources.StringResources.MakerKey);

        private Peach _peach;
        private Temperature _temperature;


        internal void Run()
        {
            var ipAddress = NetworkUtility.InitNetwork();
            if (ipAddress == "0.0.0.0")
                return;

            _peach = new Peach();
            _peach.Button.ButtonPressed += Button_ButtonPressed;

            _temperature = (new SensorBoard()).Temperature;
            _peach.PulseDebugLed(200, 10);

            while (true) { }
        }

        private void Button_ButtonPressed(GrFamily.MainBoard.Button sender, GrFamily.MainBoard.Button.ButtonState state)
        {
            var temp = _temperature.TakeMeasurement();

            SendToIfttt(temp);
        }

        private void SendToIfttt(double temp)
        {
            var content = "{ \"value1\" : \"" + temp.ToString() + "\" }";
            var contentBytes = Encoding.UTF8.GetBytes(content);
            using (var request = WebRequest.Create(new Uri(_iftttAddress)) as HttpWebRequest)
            {
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = contentBytes.Length;

                using (var reqs = request.GetRequestStream())
                {
                    reqs.Write(contentBytes, 0, contentBytes.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Debug.Print("Sent to IFTTT [" + content + "] at " + DateTime.Now);
                        _peach.PulseDebugLed(100, 5);
                    }

                    var resContentBytes = new byte[(int)response.ContentLength];
                    using (var ress = response.GetResponseStream())
                    {
                        ress.Read(resContentBytes, 0, (int)response.ContentLength);
                        var resContentChars = Encoding.UTF8.GetChars(resContentBytes);
                        var resContent = new string(resContentChars);
                        Debug.Print(resContent);
                    }
                }
            }
        }
    }
}
