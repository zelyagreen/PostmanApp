using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostmanApplication.Interfaces
{
    public interface ISender
    {
        /// <summary>
        /// Отправляет сообщение по указанному адресу, возвращает True в случае успешной доставки
        /// </summary>
        /// <param name="message"> текст сообщения </param>
        /// <param name="address"> адрес доставки, в зависимости от реализации может содержать 
        /// имя аккаунта, телефон, e-mail и тд и тп</param>
        /// <returns>Результат доставки, True, если сообщение успешно доставлено</returns>
        Task SendTelegram(string message, string address, string userId);
        Task SendPhone(string message, string address, string userId);
        Task SendEmail(string message, string address, string userId);
    }
}
