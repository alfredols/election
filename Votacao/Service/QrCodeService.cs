using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votacao.Service.Interface;

namespace Votacao.Service
{
   public class QrCodeService : IQRCode
    {
        
        #region Public Methods

        public byte[] GetQrCodeToByte(string value)
        {
            Bitmap qrCodeBitmap = GenerateQRCode(value);
            var content = ImageToByte(qrCodeBitmap);
            return content;
        }

        #endregion

        #region Private Methods

        private static Bitmap GenerateQRCode(string value)
        {
            var qr = new QRCodeGenerator();
            QRCodeData data = qr.CreateQrCode(value, QRCodeGenerator.ECCLevel.L);
            QRCode code = new QRCode(data);
            Bitmap qrCodeBitmap = code.GetGraphic(2, "#000000", "#ffffff");
            return qrCodeBitmap;
        }

        private static byte[] ImageToByte(Bitmap img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        #endregion

    }
}
