using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace ImageEvaluatorInterfaces.BaseClasses
{
    public abstract class NamedData
    {
        public string Name { get; }
        public string Description { get; }
        public abstract Type DataType { get; }


        public NamedData(string description, string name)
        {
            Name = name;
            Description = description;
        }


        public LongNamedData AsLongNamedData()
        {
            return this as LongNamedData;
        }

        public BooleanNamedData AsBooleanNamedData()
        {
            return this as BooleanNamedData;
        }

        public DoubleNamedData AsDoubleNamedData()
        {
            return this as DoubleNamedData;
        }

        public DoubleVectorNamedData AsDoubleVectorNamedData()
        {
            return this as DoubleVectorNamedData;
        }

        public FloatNamedData AsFloatNamedData()
        {
            return this as FloatNamedData;
        }

        public DateTimeNamedData AsDateTimeNamedData()
        {
            return this as DateTimeNamedData;
        }

        public TimeSpanNamedData AsTimeSpanNamedData()
        {
            return this as TimeSpanNamedData;
        }

        public StringNamedData AsStringNamedData()
        {
            return this as StringNamedData;
        }

        public EmguByteNamedData AsEmguByteNamedData()
        {
            return this as EmguByteNamedData;
        }

        public EmguFloatNamedData AsEmguFloatNamedData()
        {
            return this as EmguFloatNamedData;
        }

        public BorderPointArraysNamedData AsBorderPointArraysNamedData()
        {
            return this as BorderPointArraysNamedData;
        }


    }


    public class NamedData<T> : NamedData
    {
        T Value { get; set; }


        public override Type DataType
        {
            get { return typeof(T); }
        }

        public NamedData(T value, string description, string name)
            : base(description, name)
        {
            Value = value;
        }


        public static implicit operator T(NamedData<T> data)
        {
            return data.Value;
        }
    }


    public sealed class LongNamedData : NamedData<long>
    {
        public LongNamedData(long value, string description, string name)
            : base(value, description, name)
        {
        }

    }

    public sealed class DoubleNamedData : NamedData<double>
    {
        public DoubleNamedData(double value, string description, string name)
            : base(value, description, name)
        {
        }
    }


    public sealed class DoubleVectorNamedData : NamedData<double[]>
    {
        public DoubleVectorNamedData(double[] value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public sealed class FloatNamedData : NamedData<float>
    {
        public FloatNamedData(float value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public sealed class BooleanNamedData : NamedData<bool>
    {
        public BooleanNamedData(bool value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public sealed class DateTimeNamedData : NamedData<DateTime>
    {
        public DateTimeNamedData(DateTime value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public sealed class TimeSpanNamedData : NamedData<TimeSpan>
    {
        public TimeSpanNamedData(TimeSpan value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public sealed class StringNamedData : NamedData<string>
    {
        public StringNamedData(string value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public sealed class EmguByteNamedData : NamedData<Image<Gray, byte>[]>
    {
        public EmguByteNamedData(Image<Gray, byte>[] value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public sealed class EmguFloatNamedData : NamedData<Image<Gray, byte>[]>
    {
        public EmguFloatNamedData(Image<Gray, byte>[] value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public sealed class BorderPointArraysNamedData : NamedData<BorderPointArrays>
    {
        public BorderPointArraysNamedData(BorderPointArrays value, string description, string name)
            : base(value, description, name)
        {
        }
    }


}
