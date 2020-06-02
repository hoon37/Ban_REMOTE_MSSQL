using System;
using System.IO;
using System.Text;

namespace nowLibrary
{
    public class ErrorLog
    {
        static string filePath = string.Empty;

        static ErrorLog()
        {
            // 로그 경로
            if (string.IsNullOrEmpty(filePath))
                filePath = System.Configuration.ConfigurationManager.AppSettings["ErrorLogPath"];
        }

        /// <summary>
        /// 로그에 남긴다.
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="ex"></param>
        public static void WriteError(string pageName, string ex)
        {
            WriteError(pageName, new Exception(ex));
        }

        /// <summary>
        /// 로그에 남긴다.
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="ex"></param>
        public static void WriteError(string pageName, Exception ex)
        {
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            string fullPath = Path.Combine(filePath, fileName.Substring(0, 4) + "\\" + fileName.Substring(4, 2));

            // 디렉토리 확인
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);


            // 로그 메시지
            StringBuilder message = new StringBuilder();
            message.Append("페이지명 : ").Append(pageName).Append(Environment.NewLine);
            message.Append("발생시간 : ").Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Append(Environment.NewLine);
            message.Append("에러내용").Append(Environment.NewLine);
            message.Append("----------------------------------------------------------------------").Append(Environment.NewLine);
            message.Append(ex.ToString()).Append(Environment.NewLine);
            message.Append("----------------------------------------------------------------------").Append(Environment.NewLine).Append(Environment.NewLine);

            StreamWriter sw = null;

            try
            {
                sw = File.AppendText(Path.Combine(fullPath, fileName));
                sw.WriteLine(message.ToString());
            }
            //catch (NullReferenceException ner)
            //{
            //    // 파일 로그기록시 에러난 경우 윈도우 이벤트에 로그를 남긴다.
            //    //EventLog.WriteError(ner.ToString());
            //}
            //catch (Exception e)
            //{
            //    // 파일 로그기록시 에러난 경우 윈도우 이벤트에 로그를 남긴다.
            //    //EventLog.WriteError(e.ToString());
            //}
            catch
            {
                //
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }

    }
}
