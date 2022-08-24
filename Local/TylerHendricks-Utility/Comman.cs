using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace TylerHendricks_Utility
{
    public static class Comman
    {
        private const string DocumentPath = @"wwwroot/Documents/{0}.html";
        private const string TemplatePath = @"wwwroot/EmailTemplate/{0}.html";
        private const string EncryptionKey = "MYTRTDJD12313131OPQRSTUVWXYZ12";
        public static string GetHtmlBody(string TemplateName)
        {
            string filePath = string.Format(TemplatePath, TemplateName);
            string html = File.ReadAllText(filePath);
            return html;
        }
        public static string GetFileText(string FileName)
        {
            string filePath = string.Format(DocumentPath, FileName);
            string html = File.ReadAllText(filePath);
            return html;
        }
        public static string UpdatePaceHolder(string HtmlBody, List<KeyValuePair<string, string>> keyValuePair)
        {
            if (!string.IsNullOrEmpty(HtmlBody) && keyValuePair != null)
            {
                foreach (var item in keyValuePair)
                {
                    if (HtmlBody.Contains(item.Key))
                    {
                        HtmlBody = HtmlBody.Replace(item.Key, item.Value);
                    }
                }
            }
            return HtmlBody;
        }
        public static string CalculateYourAge(DateTime Dob)
        {
            DateTime Now = DateTime.Now;
            int Years = new DateTime(DateTime.Now.Subtract(Dob).Ticks).Year - 1;
            DateTime PastYearDate = Dob.AddYears(Years);
            int Months = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (PastYearDate.AddMonths(i) == Now)
                {
                    Months = i;
                    break;
                }
                else if (PastYearDate.AddMonths(i) >= Now)
                {
                    Months = i - 1;
                    break;
                }
            }
            int Days = Now.Subtract(PastYearDate.AddMonths(Months)).Days;
            int Hours = Now.Subtract(PastYearDate).Hours;
            int Minutes = Now.Subtract(PastYearDate).Minutes;
            int Seconds = Now.Subtract(PastYearDate).Seconds;
            return Years.ToString();
            // return String.Format("Age: {0} Year(s) {1} Month(s) {2} Day(s) {3} Hour(s) {4} Second(s)",Years, Months, Days, Hours, Seconds);
        }
        public static string Encrypt(string encryptString)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
                pdb.Dispose();
            }
            return encryptString;
        }
        public static string Decrypt(string cipherText)
        {
            if (!string.IsNullOrEmpty(cipherText))
            {
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                    pdb.Dispose();
                }
            }
            return cipherText;
        }
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
        public static DateTime GetUTCfromUserDateTime(DateTime date, int timeZone)
        {
            date = date.AddMinutes(timeZone);
            return date;
        }
        public static DateTime GetUserDateByTimeZone(DateTime date, int TimeZone, bool? isDayLightSaving)
        {
            int TimeZoneHours = (TimeZone * -1) / 60;
            int TimeZoneMinuntes = (TimeZone * -1) % 60;
            List<TimeZoneInfo> tzList = TimeZoneInfo.GetSystemTimeZones().ToList();
            var StandardNameList = tzList.Where(item => item.BaseUtcOffset.Hours == TimeZoneHours && item.BaseUtcOffset.Minutes == TimeZoneMinuntes).FirstOrDefault();
            TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(StandardNameList.StandardName);
            bool isDaylight = tst.SupportsDaylightSavingTime;
            //if (isDaylight)
            //{
            //    TimeZone = (TimeZone * -1) + 60;
            //    date = date.AddMinutes(TimeZone);
            //}
            //else
            //{
            //    TimeZone = (TimeZone * -1);
            //    date = date.AddMinutes(TimeZone);
            //}
            TimeZone = (TimeZone * -1);
            date = date.AddMinutes(TimeZone);
            return date;
        }
        public static bool IsLeapYear(int year)
        {
            if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0))
            {
                return true;
            }
            return false;
        }
        public static string GenerateOTP(int length)
        {
            char[] charArr = "0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new Random();
            for (int i = 0; i < length; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos).ToString())) strrandom += charArr.GetValue(pos);
                else i--;
            }
            return strrandom;
        }
        public static string GetLastPathSegment(this string path)
        {
            if (path != null)
            {
                string lastPathSegment = path
                .Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault();
                return lastPathSegment;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">Get File Stream</param>
        /// <param name="physicialPath">Full path of folder where file be uploaded</param>
        /// <returns>file name</returns>
        public static string UploadFile(IFormFile file, string physicialPath)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            string folderName = GetLastPathSegment(physicialPath);
            if (!Directory.Exists(physicialPath))
            {
                Directory.CreateDirectory(physicialPath);
            }
            string extension = Path.GetExtension(file.FileName);
            string fileName = "FILE" + DateTime.UtcNow.Ticks + extension;
            string sourceLocation = Path.Combine(physicialPath, fileName);
            string returnPath = "/"+folderName+"/"+fileName;
            try
            {
                using (FileStream fileStream = new FileStream(sourceLocation, FileMode.CreateNew, FileAccess.Write))
                {
                    file.CopyTo(fileStream);
                }
                return returnPath;
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static string GenerateId()
        {
            return DateTime.Now.Ticks + Guid.NewGuid().ToString("N").Substring(1, 8).ToUpper();
        }
        public static string GetTimeStamp()
        {
            var nowUTC = DateTime.UtcNow;
            var dateTime1 = new DateTime(nowUTC.Year, nowUTC.Month, nowUTC.Day, nowUTC.Hour, nowUTC.Minute, nowUTC.Second);  // 05.04.2020 12:15:12
            var UnixTimeStamp = dateTime1.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            return Convert.ToInt64(UnixTimeStamp).ToString();
        }
        public static string GetHashedMessage(string plainText, string secretKey)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secretKey);
            HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);
            byte[] messageBytes = encoding.GetBytes(plainText);
            byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);
            return ByteToString(hashmessage).ToLower();
        }
        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }
        public static string TextToHtml(string text)
        {
            text = HttpUtility.HtmlEncode(text);
            text = text.Replace("\r\n", "\r");
            text = text.Replace("\n", "\r");
            text = text.Replace("\r", "<br>\r\n");
            text = text.Replace("  ", " &nbsp;");
            return text;
        }
        public static string To_Mdyyyy_WithSlash(this DateTime? strdate)
        {
            try
            {
                if (strdate == null)
                    return null;
                else
                {
                    DateTime date = Convert.ToDateTime(strdate);
                    string utcstr = date.ToString("M'/'d'/'yyyy", CultureInfo.InvariantCulture);
                    return utcstr;
                }
            }
            catch
            {
                return null;
            }
        }
        public static string To_MMddyyyy_WithSlash(this DateTime strdate)
        {
            return strdate.ToString("MM/dd/yyyy");
        }
        public static string To_MMddyyyy_hh_mm_tt_WithSlash(this DateTime dateTime)
        {
            return dateTime.ToString("MM/dd/yyyy hh:mm tt");
        }
        public static string To_MMddyyyy_hh_mm_tt_WithSlash(this DateTime? dateTime)
        {
            try
            {
                if (dateTime == null)
                    return null;
                else
                {
                    DateTime date = Convert.ToDateTime(dateTime);
                    string utcstr = date.ToString("MM/dd/yyyy hh:mm tt");
                    return utcstr;
                }
            }
            catch
            {
                return null;
            }
        }
        public static string ConvertToBase64(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            return Convert.ToBase64String(bytes);
        }
    }
}
