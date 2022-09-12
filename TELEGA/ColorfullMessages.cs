using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TELEGA
{
    public class ColorfullMessages
    {    
        public void DisplayRedMessage(string message)
        {
            // Устанавливаем красный цвет символов
         
            Console.ForegroundColor = ConsoleColor.Red;
            
            Console.WriteLine(message);
            // Сбрасываем настройки цвета
            Console.ResetColor();
        }
        public void DisplayYellowMessage(string message)
        {
            // Устанавливаем красный цвет символов
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            // Сбрасываем настройки цвета
            Console.ResetColor();
        }
        public void DisplayGreenMessage(string message)
        {
            // Устанавливаем красный цвет символов
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            // Сбрасываем настройки цвета
            Console.ResetColor();
        }
    }
}
