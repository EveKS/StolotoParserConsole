using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace StolotoParserConsole
{

    public class StolotoResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("result")]
        public Dictionary<string, Dictionary<string, Result[]>>[] Result { get; set; }
    }

    public class Result
    {
        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("isJP")]
        public Nullable<bool> IsJp { get; set; }

        [JsonProperty("jackpotSum")]
        public long JackpotSum { get; set; }

        [JsonProperty("nextJackpotSum")]
        public long NextJackpotSum { get; set; }

        [JsonProperty("extraData")]
        public ExtraData ExtraData { get; set; }

        [JsonProperty("counts")]
        public long[] Counts { get; set; }

        [JsonProperty("numbers")]
        public string[] Numbers { get; set; }

        [JsonProperty("prizeFundRest")]
        public long PrizeFundRest { get; set; }

        [JsonProperty("externalId")]
        public long ExternalId { get; set; }

        [JsonProperty("totalPrize")]
        public long TotalPrize { get; set; }

        [JsonProperty("sumVal")]
        public long SumVal { get; set; }

        [JsonProperty("prizes")]
        public long[] Prizes { get; set; }

        [JsonProperty("jackpots")]
        public long[] Jackpots { get; set; }
    }

    public class ExtraData
    {
        [JsonProperty("nextJackpot")]
        public long NextJackpot { get; set; }

        [JsonProperty("game")]
        public string Game { get; set; }

        [JsonProperty("drawnum")]
        public long Drawnum { get; set; }

        [JsonProperty("nextDrawNumber")]
        public long NextDrawNumber { get; set; }

        [JsonProperty("jackpot")]
        public long Jackpot { get; set; }

        [JsonProperty("columns")]
        public long Columns { get; set; }

        [JsonProperty("players")]
        public long Players { get; set; }

        [JsonProperty("prizeFundRest")]
        public long PrizeFundRest { get; set; }

        [JsonProperty("categories")]
        public Category[] Categories { get; set; }

        [JsonProperty("winSuperPrize")]
        public long WinSuperPrize { get; set; }

        [JsonProperty("totalPrize")]
        public long TotalPrize { get; set; }
    }

    public class Category
    {
        [JsonProperty("win_columns_number")]
        public long WinColumnsNumber { get; set; }

        [JsonProperty("wincomb")]
        public object[] Wincomb { get; set; }

        [JsonProperty("lotos_wcnumber")]
        public long LotosWcnumber { get; set; }

        [JsonProperty("wcnumber")]
        public long Wcnumber { get; set; }

        [JsonProperty("dividend_per_column")]
        public long DividendPerColumn { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("jackpot_prev")]
        public long JackpotPrev { get; set; }

        [JsonProperty("altprize")]
        public string Altprize { get; set; }

        [JsonProperty("win_tickets")]
        public long WinTickets { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();

            var url = @"https://www.lotonews.ru/spa/get-draw-archive?game=5x36&from=1&to=1000&firstDraw=1&lastDraw=1000&mode=draw";

            var json = program.GetStringContent(url);

            var stolotoResult = JsonConvert.DeserializeObject<StolotoResult>(json);

            var filePath = "5-36.txt";

            program.ClearFile(filePath);

            program.WriteStolotoResult(stolotoResult, filePath);

            Console.WriteLine("Writed");

            Console.ReadKey();
        }

        private void SetHeaders(WebClient client)
        {
            client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
        }

        private string GetStringContent(string url)
        {
            using (WebClient client = new WebClient())
            {
                this.SetHeaders(client);

                return client.UploadString(url, "");
            }
        }

        private void WriteStolotoResult(StolotoResult stolotoResult, string filePath)
        {
            var format = "{0} _ {1}";

            this.WriteInfo(stolotoResult, format, filePath);
        }

        private void WriteInfo(StolotoResult stolotoResult, string format, string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default))
            {
                foreach (var stolotoParseResult in stolotoResult.Result)
                {
                    foreach (var itemKey in stolotoParseResult.Keys)
                    {
                        var results = stolotoParseResult[itemKey];

                        foreach (var key in results.Keys)
                        {
                            var items = results[key];

                            foreach (var item in items.OrderBy(v => v.ExternalId))
                            {
                                var draw = item.ExternalId;

                                var numbers = item.Numbers.Select(val => Convert.ToInt32(val));

                                streamWriter.WriteLine(string.Format(format, draw, string.Join(" ", numbers.Select(val => val.ToString("d2")))));
                            }
                        }
                    }
                }
            }
        }

        private void ClearFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)) { }
        }
    }
}
