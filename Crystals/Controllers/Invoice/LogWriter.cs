using System;
using System.Text;
using System.IO;

namespace Crystals.Controllers
{
    /// <Description>
    /// Provides the infomation about logging mechanism; Singleton class, cannot be instantiated more than once.
    /// </Description>
    class LogWriter
    {
        private static Object _log_lock = new Object();
        private static LogWriter _instance;
        private string _originalFileName = string.Empty;
        private int _fileIndex = 0;
        private const int _maximumFileSize = 10;     //10 MB
        private DateTime _createdDate = new DateTime();

        /// <Description>
        /// Private Constructor
        /// </Description>
        private LogWriter()
        {
            _originalFileName = getFileName(true);
        }

        /// <Description>
        /// Destructor
        /// </Description>
        ~LogWriter()
        {
            _instance = null;
        }

        /// <Description>
        /// Represents the method to write the message into the text file.
        /// </Description>
        /// <param name="fileName"></param>
        /// <param name="data2Write"></param>
        /// <returns></returns>
        private void WriteToFile(string fileName, string data2Write)
        {
            data2Write = data2Write + Environment.NewLine;
            System.IO.File.AppendAllText(fileName, data2Write);
        }

        /// <Description>
        /// Represents the method to check the file size is less than maximum size (10MB default).
        /// If the size is more than 10 MB , creates new file name with increment counter.
        /// </Description>
        /// <param name=""></param>
        /// <returns></returns>
        private void checkFileLimitation()
        {
            if (_createdDate != DateTime.Now.Date || !System.IO.File.Exists(_originalFileName) )
            {
                _originalFileName = getFileName(true);
            }

            if (System.IO.File.Exists(_originalFileName))
            {
                if (getFileSize(_originalFileName) >= _maximumFileSize)
                {
                    string newFileName = string.Empty;
                    bool isFileExist = true;
                    do
                    {
                        ++_fileIndex;
                        newFileName = getFileName(false);
                        if (File.Exists(newFileName))
                        {
                            isFileExist = true;
                        }
                        else
                        {
                            File.Move(_originalFileName, newFileName);
                            isFileExist = false;
                        }

                    } while (isFileExist);
                }
            }
        }

        /// <Description>
        /// To get log file name with location
        /// </Description>
        /// <param name="isActualName">True-> Original log file name; False-> File name along with increment counter [(e-g):XX_#] </param>
        /// <returns>File Name with file location</returns>
        private string getFileName(bool isActualName)
        {
            StringBuilder fileNameBuiler = new StringBuilder();
            _createdDate = DateTime.Now.Date;

            fileNameBuiler.Append("Log");
            fileNameBuiler.Append(DateTime.Now.Day.ToString("00")).Append(DateTime.Now.Month.ToString("00")).Append(DateTime.Now.Year.ToString());
            if (isActualName)
            {
                fileNameBuiler.Append(".Log");
            }
            else
            {
                fileNameBuiler.Append(_fileIndex > 0 ? "_" + _fileIndex.ToString() : "").Append(".Log");
            }

            return fileNameBuiler.ToString();
        }

        /// <Description>
        /// To get the file size
        /// </Description>
        /// <param name="fileName">Name & Path of file </param>
        /// <returns>Size in MB</returns>
        private double getFileSize(string fileName)
        {
            double fileSize = 0;

            FileInfo info = new FileInfo(fileName);
            fileSize = info.Length / 1048576; //1024 * 1024

            return fileSize;
        }


        /// <Description>
        /// Gets the logging instance to be used to log information to a file
        /// </Description> 
        /// <param name=""></param>
        /// <returns></returns>
        public static LogWriter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogWriter();
                }
                return _instance;
            }
        }

        /// <Description>
        /// Represents the method to log message into the log file
        /// </Description>
        /// <param name="contents"></param>
        /// <returns></returns>
        public void Log(string contents)
        {
            try
            {
                lock (_log_lock)
                {
                    checkFileLimitation();

                    contents = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + "  " + contents;

                    WriteToFile(_originalFileName, contents);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("LogWriter: Log: Failed to write log information. Exception: ", ex.ToString());
            }
        }

    }
}
