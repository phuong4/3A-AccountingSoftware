using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AAA_SODMHDB
{
    public class QRCodeUtils
    {
        public static string CreateQrCode(string bin, string consumerID, string money, string noiDung, string currencyCode = "704", string countryId = "VN")
        {
            //Phiên bản dữ liệu
            var payloadFormatIndicator = "000201";
            //Phương thức khởi tạo
            var pointOfInitiationMethod = "010212";
            //Thông tin định danh người thụ hưởng
            var consumerAccountInformation = "38";
            //Định danh tổ thức thanh toán trung gian (Napas)
            var GUID = "A000000727";//Cố định VietQR cho Napas;
            //dịch vụ chuyển nhanh   NAPAS247 bằng mã QR đến Tài khoản.
            var serviceCode = "QRIBFTTA";
            //QRIBFTTC: dịch vụ chuyển nhanh  NAPAS247 bằng mã QR đến thẻ
            // serviceCode = "QRIBFTTC";

            //Tổ chức thụ hưởng
            var organizationBeneficiaries = "00" + GetValueLength(bin) + bin;//Mã tổ chức thụ hưởng
            organizationBeneficiaries += "01" + GetValueLength(consumerID) + consumerID;//Số tài khoản của khách mở tại tổ chức thụ hưởng

            var consumerAccountInformationData = "00" + GetValueLength(GUID) + GUID + "01" + GetValueLength(organizationBeneficiaries) + organizationBeneficiaries;
            consumerAccountInformationData += "02" + GetValueLength(serviceCode) + serviceCode;//Mã dịch vụ thanh toán (Thanh toán tới thẻ hay thanh toán tới tải khoản)

            consumerAccountInformation = "38" + GetValueLength(consumerAccountInformationData) + consumerAccountInformationData;
            //Mã tiền tệ giao dichj
            var transactionCurrency = "5303" + currencyCode;
            var moneyPart = "54" + GetValueLength(money) + money;
            var countryCode = "58" + GetValueLength(countryId) + countryId;
            var purposeOfTransaction = "08" + GetValueLength(noiDung) + noiDung;
            var extenInfo = "62" + GetValueLength(purposeOfTransaction) + purposeOfTransaction;


            var qrCodeData = payloadFormatIndicator + pointOfInitiationMethod + consumerAccountInformation + transactionCurrency + moneyPart + countryCode + extenInfo;
            //cRc prefix;
            qrCodeData += "6304";
            var cRC = CalcCRC16(Encoding.ASCII.GetBytes(qrCodeData));
            qrCodeData += cRC;
            return qrCodeData;
        }
        public static string GetValueLength(string v)
        {
            return v.Length.ToString().PadLeft(2, '0');
        }
        public static string CalcCRC16(byte[] data)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)(data[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            return crc.ToString("X4");
        }
    }
}
