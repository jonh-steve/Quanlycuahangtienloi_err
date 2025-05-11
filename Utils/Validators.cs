// File: Utils/Validators.cs (Class)
using System;
using System.Text.RegularExpressions;

namespace QuanLyCuaHangTienLoi.Utils
{
    public static class Validators
    {
        // Kiểm tra email hợp lệ
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Sử dụng Regex cơ bản để kiểm tra định dạng email
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        // Kiểm tra số điện thoại hợp lệ
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Chấp nhận số điện thoại có 10-11 chữ số, có thể có dấu + ở đầu
            string pattern = @"^\+?[0-9]{10,11}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        // Kiểm tra mật khẩu đủ mạnh
        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            // Mật khẩu phải có ít nhất 8 ký tự, bao gồm: chữ hoa, chữ thường, số, ký tự đặc biệt
            bool hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            bool hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            bool hasDigit = Regex.IsMatch(password, @"[0-9]");
            bool hasSpecialChar = Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }

        // Kiểm tra mã sản phẩm hợp lệ
        public static bool IsValidProductCode(string productCode)
        {
            if (string.IsNullOrWhiteSpace(productCode))
                return false;

            // Mã sản phẩm chỉ chứa chữ cái, số và dấu gạch dưới, tối thiểu 3 ký tự
            string pattern = @"^[a-zA-Z0-9_-]{3,20}$";
            return Regex.IsMatch(productCode, pattern);
        }

        // Kiểm tra barcode hợp lệ
        public static bool IsValidBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return true; // Barcode có thể không có

            // Barcode EAN-13 (13 chữ số)
            if (barcode.Length == 13 && Regex.IsMatch(barcode, @"^\d{13}$"))
                return true;

            // Barcode EAN-8 (8 chữ số)
            if (barcode.Length == 8 && Regex.IsMatch(barcode, @"^\d{8}$"))
                return true;

            // Barcode UPC-A (12 chữ số)
            if (barcode.Length == 12 && Regex.IsMatch(barcode, @"^\d{12}$"))
                return true;

            // Barcode UPC-E (6 chữ số)
            if (barcode.Length == 6 && Regex.IsMatch(barcode, @"^\d{6}$"))
                return true;

            // Barcode Code 39 (độ dài thay đổi, chữ cái, số và một số ký tự đặc biệt)
            if (Regex.IsMatch(barcode, @"^[A-Z0-9 $%+-./:]*$"))
                return true;

            return false;
        }

        // Kiểm tra số tiền hợp lệ
        public static bool IsValidMoney(decimal money)
        {
            return money >= 0 && money <= decimal.MaxValue;
        }

        // Kiểm tra số lượng hợp lệ
        public static bool IsValidQuantity(int quantity)
        {
            return quantity >= 0;
        }
    }
}