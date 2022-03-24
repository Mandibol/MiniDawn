using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Stat<T> 
{
    public T Value { get; set; }
    public int Count { get; set; }

    public Stat(T Value, int Count)
    {
        this.Value = Value;
        this.Count = Count;
    }
}

