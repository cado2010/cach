using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cachBot
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    // botsimpson: new TgBot("129592802:AAHwLqctdxJ6yreWlUoYm-xzTHpfApYjWd0").Run();
                    // new TgBot("351786904:AAF0jZwxV0ly3UQ8tyesoZxxNhCzmqcOeXw").Run();
                    new CachBot("351786904:AAF0jZwxV0ly3UQ8tyesoZxxNhCzmqcOeXw");
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
