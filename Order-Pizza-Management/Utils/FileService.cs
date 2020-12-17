using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Order_Pizza_Management.Utils
{
    class FileService
    {
        public void WriteReportData(string path, FullReportData data, string title)
        {
            var csv = new StringBuilder();
            csv.AppendLine(title);
            csv.AppendLine("Номер заказа;Дата создания заказа;Состав заказа;Стоимость заказа");
            foreach (ReportOrderData item in data.OrderData)
                csv.AppendLine($"{item.Id};{item.CreatedAtShortDate};{item.OrderComposition};{item.Cost}");
            csv.AppendLine("Дополнительная информация");
            csv.AppendLine($"Количество заказов;{data.OrderCount}");
            csv.AppendLine($"Общая стоимость;{data.SumCost}");
            csv.AppendLine($"Минимальная стоимость заказа;{data.MinCost}");
            csv.AppendLine($"Максимальная стоимость заказа;{data.MaxCost}");

            File.WriteAllText(path, csv.ToString(), Encoding.UTF8);
        }
    }

    public class ReportOrderData
    {
        public int Id { get; set; }
        public string CreatedAtShortDate { get; set; }
        public string OrderComposition { get; set; }
        public double Cost { get; set; }
    }

    public class FullReportData
    {
        public List<ReportOrderData> OrderData { get; set; }
        public int OrderCount { get; set; }
        public double MaxCost { get; set; }
        public double MinCost { get; set; }
        public double SumCost { get; set; }
    }
}

