using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relaxing_Kaola
{
    public interface PaymentProcessor
    {
        bool ProcessPayment(int orderId, double amount);
        bool RefundPayment(int paymentId);
    }

}
