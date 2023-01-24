﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class SyncItemList<T> : GuidItemList<T> where T : class, ISyncItem
    {
        public T? this[string path] =>
            this.FirstOrDefault(x => x.Name == path);
    }
}
