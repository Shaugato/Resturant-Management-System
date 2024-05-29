using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public interface PaymentProcessor
    {
        bool ProcessPayment(string customerName, double amount, string cardNumber);
        bool RefundPayment(int paymentId);
    }
}
