using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using NUnit.Framework;

namespace XClip.Tests
{
    public abstract class ApiControllerTestBase
    {
        private readonly NetworkCredential _credentials = null;
        protected readonly List<string> _errors = new List<string>();

        private const string NULL = "[NULL]";

        protected ApiControllerTestBase()
        {
            //_credentials = new NetworkCredential(Config.Username, Config.Password);
        }

        private readonly List<TestResult> _results = new List<TestResult>();
        private readonly bool _logResults = false;

        [TestFixtureSetUp]
        public void SetUp()
        {
            //_results = new List<TestResult>();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            //// sort all results by BytesPerSecond to find slow calls
            //var sorted = new SortedDictionary<double, TestResult>();

            //_results.ForEach(x => {
            //                          if (!sorted.ContainsKey(x.BytesPerSecond))
            //                          {
            //                              sorted.Add(x.BytesPerSecond, x);
            //                          }
            //});

            //var idx = 1;
            //sorted.ToList().ForEach(x =>
            //{
            //    var r = x.Value;
            //    Console.WriteLine("{0}\t{1}\t{2}s\t{3}b\t{4}b/s\t{5}", idx, r.Url, r.TotalSeconds, r.TotalBytes, r.BytesPerSecond, r.Method);
            //    idx++;
            //});
        }

        private T PerformGet<T>(string uri)
        {
            var json = PerformGet(uri);
            return JsonConvert.DeserializeObject<T>(json);
        }

        private string PerformGet(string uri)
        {
            using (var webClient = new WebClient())
            {
                webClient.Credentials = _credentials;

                try
                {
                    using (var stream = webClient.OpenRead(uri))
                    {
                        if (stream != null)
                            using (var streamReader = new StreamReader(stream))
                            {
                                return streamReader.ReadToEnd();
                            }
                    }
                }
                catch (WebException)
                {
                    Console.WriteLine(uri);
                    throw;
                }
            }
            return string.Empty;
        }

        protected T PerformGet<T>(string uri, bool measure = false)
        {
            if (!measure)
                return PerformGet<T>(uri);

            var data = string.Empty;
            var responseLength = 0L;

            var elapsed = TimeKeeper.Measure(() =>
            {
                // This code uses a different version of PerformGet(string uri) due to the fact that
                // it is measuring the data size retrieved
                // Other wise the code being measured would be the same
                // Can this adversely affect testing?
                using (var webClient = new WebClient())
                {
                    webClient.Credentials = _credentials;

                    try
                    {
                        using (var stream = webClient.OpenRead(uri))
                        {
                            if (stream == null) return;

                            using (var ms = new MemoryStream())
                            {
                                stream.CopyTo(ms);
                                responseLength = ms.Length;
                                data = Encoding.Default.GetString((ms.ToArray()));
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        Console.WriteLine(uri);
                        Console.WriteLine(ex.ToString());
                        throw;
                    }
                }
            });

            var ratio = Math.Round(responseLength / elapsed.TotalSeconds, 2);
            var result = new TestResult
            {
                Url = uri,
                TotalSeconds = Math.Round(elapsed.TotalSeconds, 2),
                TotalBytes = responseLength,
                BytesPerSecond = ratio,
                Method = "GET"
            };
            _results.Add(result);

            if (_logResults)
                LogResult(result);

            return JsonConvert.DeserializeObject<T>(data);
        }

        protected string PerformGet(string uri, bool measure = false)
        {
            if (!measure)
                return PerformGet(uri);

            var data = string.Empty;
            var responseLength = 0L;

            var elapsed = TimeKeeper.Measure(() =>
            {
                // This code uses a different version of PerformGet(string uri) due to the fact that
                // it is measuring the data size retrieved
                // Other wise the code being measured would be the same
                // Can this adversely affect testing?
                using (var webClient = new WebClient())
                {
                    webClient.Credentials = _credentials;

                    try
                    {
                        using (var stream = webClient.OpenRead(uri))
                        {
                            if (stream == null) return;

                            using (var ms = new MemoryStream())
                            {
                                stream.CopyTo(ms);
                                responseLength = ms.Length;
                                data = Encoding.Default.GetString((ms.ToArray()));
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        Console.WriteLine(uri);
                        Console.WriteLine(ex.ToString());
                        throw;
                    }
                }
            });

            var ratio = Math.Round(responseLength / elapsed.TotalSeconds, 2);
            var result = new TestResult
            {
                Url = uri,
                TotalSeconds = Math.Round(elapsed.TotalSeconds, 2),
                TotalBytes = responseLength,
                BytesPerSecond = ratio,
                Method = "GET"
            };
            _results.Add(result);

            if (_logResults)
                LogResult(result);

            return data;

        }

        /// <summary>
        /// http://stackoverflow.com/questions/5401501/how-to-post-data-to-specific-url-using-webclient-in-c-sharp
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="jsonData"></param>
        /// <param name="measure"></param>
        /// <returns></returns>
        protected string PerformPost(string uri, string jsonData, bool measure = false)
        {
            if (!measure)
                return PerformPostOrPut(true, new Uri(uri), jsonData);

            var data = string.Empty;
            var responseLength = 0L;

            var ratio = 0D;

            var elapsed = TimeKeeper.Measure(() =>
            {
                data = PerformPostOrPut(true, new Uri(uri), jsonData);
            });
            ratio = Math.Round(responseLength / elapsed.TotalSeconds, 2);

            var result = new TestResult
            {
                Url = uri,
                TotalSeconds = Math.Round(elapsed.TotalSeconds, 2),
                TotalBytes = responseLength,
                BytesPerSecond = ratio > 0D ? ratio : 0,
                Method = "POST"
            };
            _results.Add(result);

            if (_logResults)
                LogResult(result);

            return data;

        }

        protected string PerformPut(string uri, string jsonData, bool measure = false)
        {
            if (!measure)
                return PerformPostOrPut(false, new Uri(uri), jsonData);

            var data = string.Empty;
            var responseLength = 0L;

            var ratio = 0D;

            var elapsed = TimeKeeper.Measure(() =>
            {
                data = PerformPostOrPut(false, new Uri(uri), jsonData);
            });
            ratio = Math.Round(responseLength / elapsed.TotalSeconds, 2);

            var result = new TestResult
            {
                Url = uri,
                TotalSeconds = Math.Round(elapsed.TotalSeconds, 2),
                TotalBytes = responseLength,
                BytesPerSecond = ratio > 0D ? ratio : 0,
                Method = "PUT"
            };
            _results.Add(result);

            if (_logResults)
                LogResult(result);

            return data;
        }

        private string PerformPostOrPut(bool isPost, Uri uri, string jsonData)
        {
            var method = isPost ? "POST" : "PUT";
            using (var client = new WebClient())
            {

                client.Credentials = _credentials;

                client.Headers["Content-Type"] = "application/json";

                var postArray = Encoding.ASCII.GetBytes(jsonData);

                byte[] responsebytes = client.UploadData(uri, method, postArray);

                return Encoding.UTF8.GetString(responsebytes);
            }
        }

        /// <summary>
        /// http://stackoverflow.com/questions/8847679/find-average-of-collection-of-timespans
        /// </summary>
        /// <param name="timeSpans"></param>
        /// <returns></returns>
        protected static TimeSpan TimespanAverage(IEnumerable<TimeSpan> timeSpans)
        {

            double doubleAverageTicks = timeSpans.Average(timeSpan => timeSpan.Ticks);
            long longAverageTicks = Convert.ToInt64(doubleAverageTicks);

            return new TimeSpan(longAverageTicks);
        }

        protected void Compare(string first, string second, string name)
        {
            if (first == second) return;
            first = string.IsNullOrEmpty(first) ? NULL : first;
            second = string.IsNullOrEmpty(second) ? NULL : second;
            _errors.Add(string.Format("{0} did not match\t{1}\t{2}", name, first, second));
        }

        protected void Compare(DateTime? first, DateTime? second, string name)
        {
            if (first == second) return;
            var f = first == null ? NULL : first.ToString();
            var s = second == null ? NULL : second.ToString();
            _errors.Add(string.Format("{0} did not match\t{1}\t{2}", name, f, s));
        }

        protected void Compare(int? first, int? second, string name)
        {
            if (first == second) return;
            var f = first == null ? NULL : first.ToString();
            var s = second == null ? NULL : second.ToString();
            _errors.Add(string.Format("{0} did not match\t{1}\t{2}", name, f, s));
        }

        protected void Compare(decimal? first, decimal? second, string name)
        {
            if (first == second) return;
            var f = first == null ? NULL : first.ToString();
            var s = second == null ? NULL : second.ToString();
            _errors.Add(string.Format("{0} did not match\t{1}\t{2}", name, f, s));
        }

        protected void Compare(bool? first, bool? second, string name)
        {
            if (first == second) return;
            var f = first == null ? NULL : first.ToString();
            var s = second == null ? NULL : second.ToString();
            _errors.Add(string.Format("{0} did not match\t{1}\t{2}", name, f, s));
        }

        protected void Compare(string[] first, string[] second, string name)
        {
            var fList = first == null ? new List<string>() : first.ToList();
            var sList = first == null ? new List<string>() : second.ToList();

            fList.ForEach(x =>
            {
                if (sList.Contains(x))
                    return;
                _errors.Add(string.Format("{0} secondList did not contain\t{1}", name, x));
            });

            sList.ForEach(x => {
                if (fList.Contains(x))
                    return;
                _errors.Add(string.Format("{0} firstList did not contain\t{1}", name, x));
            });
        }

        private void LogResult(TestResult result)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("INSERT INTO [TestResults] ([Url], [Seconds], [Bytes], [Method])");
            sql.AppendLine("VALUES (@Url, @Sec, @B, @M)");

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@Url", SqlDbType.NVarChar) {Value = result.Url},
                new SqlParameter("@Sec", SqlDbType.Decimal) {Value = result.TotalSeconds},
                new SqlParameter("@B", SqlDbType.BigInt) {Value = result.TotalBytes},
                new SqlParameter("@M", SqlDbType.NVarChar) {Value = result.Method}
            };

            using (var connection = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Projects\WorkCenters\WCTestRunner\WCTestRunner\Database1.mdf;Integrated Security=True"))
            {

                using (var cmdSql = new SqlCommand(sql.ToString(), connection))
                {

                    cmdSql.CommandTimeout = 3000;
                    cmdSql.CommandType = CommandType.Text;

                    foreach (var p in paramList) { cmdSql.Parameters.Add(p); }

                    try
                    {
                        connection.Open();
                        cmdSql.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

            }
        }

    }
}