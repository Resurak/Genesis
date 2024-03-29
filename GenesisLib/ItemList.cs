﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib
{
    public class ItemList<T> : List<T> where T : class, IGuidItem
    {
        public ItemList()
        {

        }

        public T? this[Guid id] =>
            this.FirstOrDefault(x => x.ID == id);
    }
}
