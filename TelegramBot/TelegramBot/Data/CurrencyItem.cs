namespace TelegramBot.Data
{
    /// <summary>
    /// Here we declare object properties, depending on the data obtained from https://bank.gov.ua/ API  
    /// </summary>
    public class CurrencyItem
    {
        /// <summary>
        ///Currency code
        /// </summary>
        public string r030 { get; set; }
        /// <summary>
        /// Currency name at UA
        /// </summary>
        public string txt { get; set; }
        /// <summary>
        /// Exchange rate at UAH
        /// </summary>
        public decimal rate { get; set; }
        /// <summary>
        /// Currency name
        /// </summary>
        public string cc { get; set; }
        /// <summary>
        /// Exchange data
        /// </summary>
        public string exchangedate { get; set; }
    }
}
