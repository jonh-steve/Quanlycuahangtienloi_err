// File: /Services/PaymentProcessor.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using QuanLyCuaHangTienLoi.Db;
namespace QuanLyCuaHangTienLoi.Services
{
    public class PaymentProcessor
    {
        // Enum cho các phương thức thanh toán
        public enum PaymentMethod
        {
            Cash,
            Card,
            EWallet,
            BankTransfer
        }

        // Lưu trữ các phương thức thanh toán có sẵn
        private static readonly Dictionary<int, string> _paymentMethods = new Dictionary<int, string>
        {
            { 1, "Tiền mặt" },
            { 2, "Thẻ ngân hàng" },
            { 3, "Ví điện tử" },
            { 4, "Chuyển khoản" }
        };

        // Lấy danh sách phương thức thanh toán
        public Dictionary<int, string> GetPaymentMethods()
        {
            return _paymentMethods;
        }

        // Xử lý thanh toán tiền mặt
        public bool ProcessCashPayment(decimal orderAmount, decimal amountGiven, out decimal change)
        {
            if (amountGiven < orderAmount)
            {
                change = 0;
                return false;
            }

            change = amountGiven - orderAmount;
            return true;
        }

        // Xử lý thanh toán bằng thẻ
        public bool ProcessCardPayment(string cardNumber, string cardHolderName, string expiryDate, string cvv, decimal amount)
        {
            // Trong ứng dụng thật, đây sẽ là integration với cổng thanh toán
            // Giả lập thanh toán thành công
            return ValidateCardDetails(cardNumber, expiryDate, cvv);
        }

        // Xử lý thanh toán qua ví điện tử
        public bool ProcessEWalletPayment(string walletType, string phoneNumber, decimal amount)
        {
            // Trong ứng dụng thật, đây sẽ là integration với ví điện tử
            // Giả lập thanh toán thành công
            return !string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length >= 10;
        }

        // Kiểm tra thông tin thẻ
        private bool ValidateCardDetails(string cardNumber, string expiryDate, string cvv)
        {
            // Kiểm tra số thẻ (phải có 16 số)
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length != 16 || !cardNumber.All(char.IsDigit))
                return false;

            // Kiểm tra ngày hết hạn (định dạng MM/YY)
            if (string.IsNullOrEmpty(expiryDate) || expiryDate.Length != 5 || expiryDate[2] != '/')
                return false;

            // Kiểm tra CVV (phải có 3 số)
            if (string.IsNullOrEmpty(cvv) || cvv.Length != 3 || !cvv.All(char.IsDigit))
                return false;

            return true;
        }

        // In hóa đơn
        public bool PrintReceipt(int orderID, string orderCode, DateTime orderDate,
            string customerName, string employeeName, decimal totalAmount,
            decimal tax, decimal discount, decimal finalAmount,
            List<Tuple<string, int, decimal, decimal>> items, // Tên sản phẩm, số lượng, đơn giá, thành tiền
            string paymentMethod, decimal amountPaid, decimal change)
        {
            try
            {
                // Trong ứng dụng thật, đây sẽ tích hợp với máy in
                // Hiện tại chỉ giả lập in hóa đơn

                // Tạo nội dung hóa đơn
                string receiptContent = "=========================================\n";
                receiptContent += "            CỬA HÀNG TIỆN LỢI             \n";
                receiptContent += "=========================================\n";
                receiptContent += $"Mã hóa đơn: {orderCode}\n";
                receiptContent += $"Ngày: {orderDate:dd/MM/yyyy HH:mm:ss}\n";
                receiptContent += $"Nhân viên: {employeeName}\n";

                if (!string.IsNullOrEmpty(customerName))
                    receiptContent += $"Khách hàng: {customerName}\n";

                receiptContent += "=========================================\n";
                receiptContent += "Sản phẩm          SL    Đơn giá    Thành tiền\n";

                foreach (var item in items)
                {
                    receiptContent += $"{item.Item1,-18} {item.Item2,2}  {item.Item3,10:#,##0}  {item.Item4,10:#,##0}\n";
                }

                receiptContent += "=========================================\n";
                receiptContent += $"Tổng tiền hàng:             {totalAmount,14:#,##0}\n";

                if (discount > 0)
                    receiptContent += $"Giảm giá:                  {discount,14:#,##0}\n";

                receiptContent += $"Thuế ({tax / totalAmount * 100:0}%):                  {tax,14:#,##0}\n";
                receiptContent += $"Thành tiền:                {finalAmount,14:#,##0}\n";
                receiptContent += $"Phương thức thanh toán: {paymentMethod}\n";

                if (paymentMethod == "Tiền mặt")
                {
                    receiptContent += $"Khách đưa:                 {amountPaid,14:#,##0}\n";
                    receiptContent += $"Tiền thừa:                 {change,14:#,##0}\n";
                }

                receiptContent += "=========================================\n";
                receiptContent += "         Cảm ơn quý khách đã mua hàng!        \n";
                receiptContent += "              Hẹn gặp lại quý khách           \n";
                receiptContent += "=========================================\n";

                // Hiển thị preview hóa đơn trước khi in (trong ứng dụng thật)
                // In hóa đơn ra máy in (trong ứng dụng thật)

                return true;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Logger.Log(ex);
                return false;
            }
        }
    }
}