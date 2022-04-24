using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAdmin.Store
{
    public class CounterStore : StoreBase
    {
        public int Count { get; set; }
        public SelectItem<string> Items { get; set; }
        public CounterStore()
        {
            Init();
        }

        private async void Init()
        {
            await Task.Delay(1000);
            Items = new SelectItem<string>();
            Items.Add("k1", "k1");
            Items.Add("k2", "k2");
            Items.Add("k3", "k3");
            Items.Add("k4", "k4");
        }
    }
}
