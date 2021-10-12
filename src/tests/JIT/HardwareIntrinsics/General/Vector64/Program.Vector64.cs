// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace JIT.HardwareIntrinsics.General
{
    public static partial class Program
    {
        static Program()
        {
            TestList = new Dictionary<string, Action>() {
                ["Abs.Byte"] = AbsByte,
                ["Abs.Double"] = AbsDouble,
                ["Abs.Int16"] = AbsInt16,
                ["Abs.Int32"] = AbsInt32,
                ["Abs.Int64"] = AbsInt64,
                ["Abs.SByte"] = AbsSByte,
                ["Abs.Single"] = AbsSingle,
                ["Abs.UInt16"] = AbsUInt16,
                ["Abs.UInt32"] = AbsUInt32,
                ["Abs.UInt64"] = AbsUInt64,
                ["Add.Byte"] = AddByte,
                ["Add.Double"] = AddDouble,
                ["Add.Int16"] = AddInt16,
                ["Add.Int32"] = AddInt32,
                ["Add.Int64"] = AddInt64,
                ["Add.SByte"] = AddSByte,
                ["Add.Single"] = AddSingle,
                ["Add.UInt16"] = AddUInt16,
                ["Add.UInt32"] = AddUInt32,
                ["Add.UInt64"] = AddUInt64,
                ["AndNot.Byte"] = AndNotByte,
                ["AndNot.Double"] = AndNotDouble,
                ["AndNot.Int16"] = AndNotInt16,
                ["AndNot.Int32"] = AndNotInt32,
                ["AndNot.Int64"] = AndNotInt64,
                ["AndNot.SByte"] = AndNotSByte,
                ["AndNot.Single"] = AndNotSingle,
                ["AndNot.UInt16"] = AndNotUInt16,
                ["AndNot.UInt32"] = AndNotUInt32,
                ["AndNot.UInt64"] = AndNotUInt64,
                ["BitwiseAnd.Byte"] = BitwiseAndByte,
                ["BitwiseAnd.Double"] = BitwiseAndDouble,
                ["BitwiseAnd.Int16"] = BitwiseAndInt16,
                ["BitwiseAnd.Int32"] = BitwiseAndInt32,
                ["BitwiseAnd.Int64"] = BitwiseAndInt64,
                ["BitwiseAnd.SByte"] = BitwiseAndSByte,
                ["BitwiseAnd.Single"] = BitwiseAndSingle,
                ["BitwiseAnd.UInt16"] = BitwiseAndUInt16,
                ["BitwiseAnd.UInt32"] = BitwiseAndUInt32,
                ["BitwiseAnd.UInt64"] = BitwiseAndUInt64,
                ["BitwiseOr.Byte"] = BitwiseOrByte,
                ["BitwiseOr.Double"] = BitwiseOrDouble,
                ["BitwiseOr.Int16"] = BitwiseOrInt16,
                ["BitwiseOr.Int32"] = BitwiseOrInt32,
                ["BitwiseOr.Int64"] = BitwiseOrInt64,
                ["BitwiseOr.SByte"] = BitwiseOrSByte,
                ["BitwiseOr.Single"] = BitwiseOrSingle,
                ["BitwiseOr.UInt16"] = BitwiseOrUInt16,
                ["BitwiseOr.UInt32"] = BitwiseOrUInt32,
                ["BitwiseOr.UInt64"] = BitwiseOrUInt64,
                ["Ceiling.Double"] = CeilingDouble,
                ["Ceiling.Single"] = CeilingSingle,
                ["ConditionalSelect.Byte"] = ConditionalSelectByte,
                ["ConditionalSelect.Double"] = ConditionalSelectDouble,
                ["ConditionalSelect.Int16"] = ConditionalSelectInt16,
                ["ConditionalSelect.Int32"] = ConditionalSelectInt32,
                ["ConditionalSelect.Int64"] = ConditionalSelectInt64,
                ["ConditionalSelect.SByte"] = ConditionalSelectSByte,
                ["ConditionalSelect.Single"] = ConditionalSelectSingle,
                ["ConditionalSelect.UInt16"] = ConditionalSelectUInt16,
                ["ConditionalSelect.UInt32"] = ConditionalSelectUInt32,
                ["ConditionalSelect.UInt64"] = ConditionalSelectUInt64,
                ["ConvertToDouble.Double"] = ConvertToDoubleDouble,
                ["ConvertToDouble.Double"] = ConvertToDoubleDouble,
                ["ConvertToInt32.Int32"] = ConvertToInt32Int32,
                ["ConvertToInt64.Int64"] = ConvertToInt64Int64,
                ["ConvertToSingle.Single"] = ConvertToSingleSingle,
                ["ConvertToSingle.Single"] = ConvertToSingleSingle,
                ["ConvertToUInt32.UInt32"] = ConvertToUInt32UInt32,
                ["ConvertToUInt64.UInt64"] = ConvertToUInt64UInt64,
                ["Create.Byte"] = CreateByte,
                ["Create.Double"] = CreateDouble,
                ["Create.Int16"] = CreateInt16,
                ["Create.Int32"] = CreateInt32,
                ["Create.Int64"] = CreateInt64,
                ["Create.SByte"] = CreateSByte,
                ["Create.Single"] = CreateSingle,
                ["Create.UInt16"] = CreateUInt16,
                ["Create.UInt32"] = CreateUInt32,
                ["Create.UInt64"] = CreateUInt64,
                ["CreateScalar.Byte"] = CreateScalarByte,
                ["CreateScalar.Double"] = CreateScalarDouble,
                ["CreateScalar.Int16"] = CreateScalarInt16,
                ["CreateScalar.Int32"] = CreateScalarInt32,
                ["CreateScalar.Int64"] = CreateScalarInt64,
                ["CreateScalar.SByte"] = CreateScalarSByte,
                ["CreateScalar.Single"] = CreateScalarSingle,
                ["CreateScalar.UInt16"] = CreateScalarUInt16,
                ["CreateScalar.UInt32"] = CreateScalarUInt32,
                ["CreateScalar.UInt64"] = CreateScalarUInt64,
                ["CreateScalarUnsafe.Byte"] = CreateScalarUnsafeByte,
                ["CreateScalarUnsafe.Int16"] = CreateScalarUnsafeInt16,
                ["CreateScalarUnsafe.Int32"] = CreateScalarUnsafeInt32,
                ["CreateScalarUnsafe.SByte"] = CreateScalarUnsafeSByte,
                ["CreateScalarUnsafe.Single"] = CreateScalarUnsafeSingle,
                ["CreateScalarUnsafe.UInt16"] = CreateScalarUnsafeUInt16,
                ["CreateScalarUnsafe.UInt32"] = CreateScalarUnsafeUInt32,
                ["CreateElement.Byte"] = CreateElementByte,
                ["CreateElement.Int16"] = CreateElementInt16,
                ["CreateElement.Int32"] = CreateElementInt32,
                ["CreateElement.SByte"] = CreateElementSByte,
                ["CreateElement.Single"] = CreateElementSingle,
                ["CreateElement.UInt16"] = CreateElementUInt16,
                ["CreateElement.UInt32"] = CreateElementUInt32,
                ["Divide.Byte"] = DivideByte,
                ["Divide.Double"] = DivideDouble,
                ["Divide.Int16"] = DivideInt16,
                ["Divide.Int32"] = DivideInt32,
                ["Divide.Int64"] = DivideInt64,
                ["Divide.SByte"] = DivideSByte,
                ["Divide.Single"] = DivideSingle,
                ["Divide.UInt16"] = DivideUInt16,
                ["Divide.UInt32"] = DivideUInt32,
                ["Divide.UInt64"] = DivideUInt64,
                ["Dot.Byte"] = DotByte,
                ["Dot.Double"] = DotDouble,
                ["Dot.Int16"] = DotInt16,
                ["Dot.Int32"] = DotInt32,
                ["Dot.Int64"] = DotInt64,
                ["Dot.SByte"] = DotSByte,
                ["Dot.Single"] = DotSingle,
                ["Dot.UInt16"] = DotUInt16,
                ["Dot.UInt32"] = DotUInt32,
                ["Dot.UInt64"] = DotUInt64,
                ["EqualsAll.Byte"] = EqualsAllByte,
                ["EqualsAll.Double"] = EqualsAllDouble,
                ["EqualsAll.Int16"] = EqualsAllInt16,
                ["EqualsAll.Int32"] = EqualsAllInt32,
                ["EqualsAll.Int64"] = EqualsAllInt64,
                ["EqualsAll.SByte"] = EqualsAllSByte,
                ["EqualsAll.Single"] = EqualsAllSingle,
                ["EqualsAll.UInt16"] = EqualsAllUInt16,
                ["EqualsAll.UInt32"] = EqualsAllUInt32,
                ["EqualsAll.UInt64"] = EqualsAllUInt64,
                ["EqualsAny.Byte"] = EqualsAnyByte,
                ["EqualsAny.Double"] = EqualsAnyDouble,
                ["EqualsAny.Int16"] = EqualsAnyInt16,
                ["EqualsAny.Int32"] = EqualsAnyInt32,
                ["EqualsAny.Int64"] = EqualsAnyInt64,
                ["EqualsAny.SByte"] = EqualsAnySByte,
                ["EqualsAny.Single"] = EqualsAnySingle,
                ["EqualsAny.UInt16"] = EqualsAnyUInt16,
                ["EqualsAny.UInt32"] = EqualsAnyUInt32,
                ["EqualsAny.UInt64"] = EqualsAnyUInt64,
                ["Equals.Byte"] = EqualsByte,
                ["Equals.Double"] = EqualsDouble,
                ["Equals.Int16"] = EqualsInt16,
                ["Equals.Int32"] = EqualsInt32,
                ["Equals.Int64"] = EqualsInt64,
                ["Equals.SByte"] = EqualsSByte,
                ["Equals.Single"] = EqualsSingle,
                ["Equals.UInt16"] = EqualsUInt16,
                ["Equals.UInt32"] = EqualsUInt32,
                ["Equals.UInt64"] = EqualsUInt64,
                ["Floor.Double"] = FloorDouble,
                ["Floor.Single"] = FloorSingle,
                ["GreaterThanAll.Byte"] = GreaterThanAllByte,
                ["GreaterThanAll.Double"] = GreaterThanAllDouble,
                ["GreaterThanAll.Int16"] = GreaterThanAllInt16,
                ["GreaterThanAll.Int32"] = GreaterThanAllInt32,
                ["GreaterThanAll.Int64"] = GreaterThanAllInt64,
                ["GreaterThanAll.SByte"] = GreaterThanAllSByte,
                ["GreaterThanAll.Single"] = GreaterThanAllSingle,
                ["GreaterThanAll.UInt16"] = GreaterThanAllUInt16,
                ["GreaterThanAll.UInt32"] = GreaterThanAllUInt32,
                ["GreaterThanAll.UInt64"] = GreaterThanAllUInt64,
                ["GreaterThanAny.Byte"] = GreaterThanAnyByte,
                ["GreaterThanAny.Double"] = GreaterThanAnyDouble,
                ["GreaterThanAny.Int16"] = GreaterThanAnyInt16,
                ["GreaterThanAny.Int32"] = GreaterThanAnyInt32,
                ["GreaterThanAny.Int64"] = GreaterThanAnyInt64,
                ["GreaterThanAny.SByte"] = GreaterThanAnySByte,
                ["GreaterThanAny.Single"] = GreaterThanAnySingle,
                ["GreaterThanAny.UInt16"] = GreaterThanAnyUInt16,
                ["GreaterThanAny.UInt32"] = GreaterThanAnyUInt32,
                ["GreaterThanAny.UInt64"] = GreaterThanAnyUInt64,
                ["GreaterThan.Byte"] = GreaterThanByte,
                ["GreaterThan.Double"] = GreaterThanDouble,
                ["GreaterThan.Int16"] = GreaterThanInt16,
                ["GreaterThan.Int32"] = GreaterThanInt32,
                ["GreaterThan.Int64"] = GreaterThanInt64,
                ["GreaterThan.SByte"] = GreaterThanSByte,
                ["GreaterThan.Single"] = GreaterThanSingle,
                ["GreaterThan.UInt16"] = GreaterThanUInt16,
                ["GreaterThan.UInt32"] = GreaterThanUInt32,
                ["GreaterThan.UInt64"] = GreaterThanUInt64,
                ["GreaterThanOrEqualAll.Byte"] = GreaterThanOrEqualAllByte,
                ["GreaterThanOrEqualAll.Double"] = GreaterThanOrEqualAllDouble,
                ["GreaterThanOrEqualAll.Int16"] = GreaterThanOrEqualAllInt16,
                ["GreaterThanOrEqualAll.Int32"] = GreaterThanOrEqualAllInt32,
                ["GreaterThanOrEqualAll.Int64"] = GreaterThanOrEqualAllInt64,
                ["GreaterThanOrEqualAll.SByte"] = GreaterThanOrEqualAllSByte,
                ["GreaterThanOrEqualAll.Single"] = GreaterThanOrEqualAllSingle,
                ["GreaterThanOrEqualAll.UInt16"] = GreaterThanOrEqualAllUInt16,
                ["GreaterThanOrEqualAll.UInt32"] = GreaterThanOrEqualAllUInt32,
                ["GreaterThanOrEqualAll.UInt64"] = GreaterThanOrEqualAllUInt64,
                ["GreaterThanOrEqualAny.Byte"] = GreaterThanOrEqualAnyByte,
                ["GreaterThanOrEqualAny.Double"] = GreaterThanOrEqualAnyDouble,
                ["GreaterThanOrEqualAny.Int16"] = GreaterThanOrEqualAnyInt16,
                ["GreaterThanOrEqualAny.Int32"] = GreaterThanOrEqualAnyInt32,
                ["GreaterThanOrEqualAny.Int64"] = GreaterThanOrEqualAnyInt64,
                ["GreaterThanOrEqualAny.SByte"] = GreaterThanOrEqualAnySByte,
                ["GreaterThanOrEqualAny.Single"] = GreaterThanOrEqualAnySingle,
                ["GreaterThanOrEqualAny.UInt16"] = GreaterThanOrEqualAnyUInt16,
                ["GreaterThanOrEqualAny.UInt32"] = GreaterThanOrEqualAnyUInt32,
                ["GreaterThanOrEqualAny.UInt64"] = GreaterThanOrEqualAnyUInt64,
                ["GreaterThanOrEqual.Byte"] = GreaterThanOrEqualByte,
                ["GreaterThanOrEqual.Double"] = GreaterThanOrEqualDouble,
                ["GreaterThanOrEqual.Int16"] = GreaterThanOrEqualInt16,
                ["GreaterThanOrEqual.Int32"] = GreaterThanOrEqualInt32,
                ["GreaterThanOrEqual.Int64"] = GreaterThanOrEqualInt64,
                ["GreaterThanOrEqual.SByte"] = GreaterThanOrEqualSByte,
                ["GreaterThanOrEqual.Single"] = GreaterThanOrEqualSingle,
                ["GreaterThanOrEqual.UInt16"] = GreaterThanOrEqualUInt16,
                ["GreaterThanOrEqual.UInt32"] = GreaterThanOrEqualUInt32,
                ["GreaterThanOrEqual.UInt64"] = GreaterThanOrEqualUInt64,
                ["LessThanAll.Byte"] = LessThanAllByte,
                ["LessThanAll.Double"] = LessThanAllDouble,
                ["LessThanAll.Int16"] = LessThanAllInt16,
                ["LessThanAll.Int32"] = LessThanAllInt32,
                ["LessThanAll.Int64"] = LessThanAllInt64,
                ["LessThanAll.SByte"] = LessThanAllSByte,
                ["LessThanAll.Single"] = LessThanAllSingle,
                ["LessThanAll.UInt16"] = LessThanAllUInt16,
                ["LessThanAll.UInt32"] = LessThanAllUInt32,
                ["LessThanAll.UInt64"] = LessThanAllUInt64,
                ["LessThanAny.Byte"] = LessThanAnyByte,
                ["LessThanAny.Double"] = LessThanAnyDouble,
                ["LessThanAny.Int16"] = LessThanAnyInt16,
                ["LessThanAny.Int32"] = LessThanAnyInt32,
                ["LessThanAny.Int64"] = LessThanAnyInt64,
                ["LessThanAny.SByte"] = LessThanAnySByte,
                ["LessThanAny.Single"] = LessThanAnySingle,
                ["LessThanAny.UInt16"] = LessThanAnyUInt16,
                ["LessThanAny.UInt32"] = LessThanAnyUInt32,
                ["LessThanAny.UInt64"] = LessThanAnyUInt64,
                ["LessThan.Byte"] = LessThanByte,
                ["LessThan.Double"] = LessThanDouble,
                ["LessThan.Int16"] = LessThanInt16,
                ["LessThan.Int32"] = LessThanInt32,
                ["LessThan.Int64"] = LessThanInt64,
                ["LessThan.SByte"] = LessThanSByte,
                ["LessThan.Single"] = LessThanSingle,
                ["LessThan.UInt16"] = LessThanUInt16,
                ["LessThan.UInt32"] = LessThanUInt32,
                ["LessThan.UInt64"] = LessThanUInt64,
                ["LessThanOrEqualAll.Byte"] = LessThanOrEqualAllByte,
                ["LessThanOrEqualAll.Double"] = LessThanOrEqualAllDouble,
                ["LessThanOrEqualAll.Int16"] = LessThanOrEqualAllInt16,
                ["LessThanOrEqualAll.Int32"] = LessThanOrEqualAllInt32,
                ["LessThanOrEqualAll.Int64"] = LessThanOrEqualAllInt64,
                ["LessThanOrEqualAll.SByte"] = LessThanOrEqualAllSByte,
                ["LessThanOrEqualAll.Single"] = LessThanOrEqualAllSingle,
                ["LessThanOrEqualAll.UInt16"] = LessThanOrEqualAllUInt16,
                ["LessThanOrEqualAll.UInt32"] = LessThanOrEqualAllUInt32,
                ["LessThanOrEqualAll.UInt64"] = LessThanOrEqualAllUInt64,
                ["LessThanOrEqualAny.Byte"] = LessThanOrEqualAnyByte,
                ["LessThanOrEqualAny.Double"] = LessThanOrEqualAnyDouble,
                ["LessThanOrEqualAny.Int16"] = LessThanOrEqualAnyInt16,
                ["LessThanOrEqualAny.Int32"] = LessThanOrEqualAnyInt32,
                ["LessThanOrEqualAny.Int64"] = LessThanOrEqualAnyInt64,
                ["LessThanOrEqualAny.SByte"] = LessThanOrEqualAnySByte,
                ["LessThanOrEqualAny.Single"] = LessThanOrEqualAnySingle,
                ["LessThanOrEqualAny.UInt16"] = LessThanOrEqualAnyUInt16,
                ["LessThanOrEqualAny.UInt32"] = LessThanOrEqualAnyUInt32,
                ["LessThanOrEqualAny.UInt64"] = LessThanOrEqualAnyUInt64,
                ["LessThanOrEqual.Byte"] = LessThanOrEqualByte,
                ["LessThanOrEqual.Double"] = LessThanOrEqualDouble,
                ["LessThanOrEqual.Int16"] = LessThanOrEqualInt16,
                ["LessThanOrEqual.Int32"] = LessThanOrEqualInt32,
                ["LessThanOrEqual.Int64"] = LessThanOrEqualInt64,
                ["LessThanOrEqual.SByte"] = LessThanOrEqualSByte,
                ["LessThanOrEqual.Single"] = LessThanOrEqualSingle,
                ["LessThanOrEqual.UInt16"] = LessThanOrEqualUInt16,
                ["LessThanOrEqual.UInt32"] = LessThanOrEqualUInt32,
                ["LessThanOrEqual.UInt64"] = LessThanOrEqualUInt64,
                ["Max.Byte"] = MaxByte,
                ["Max.Double"] = MaxDouble,
                ["Max.Int16"] = MaxInt16,
                ["Max.Int32"] = MaxInt32,
                ["Max.Int64"] = MaxInt64,
                ["Max.SByte"] = MaxSByte,
                ["Max.Single"] = MaxSingle,
                ["Max.UInt16"] = MaxUInt16,
                ["Max.UInt32"] = MaxUInt32,
                ["Max.UInt64"] = MaxUInt64,
                ["Min.Byte"] = MinByte,
                ["Min.Double"] = MinDouble,
                ["Min.Int16"] = MinInt16,
                ["Min.Int32"] = MinInt32,
                ["Min.Int64"] = MinInt64,
                ["Min.SByte"] = MinSByte,
                ["Min.Single"] = MinSingle,
                ["Min.UInt16"] = MinUInt16,
                ["Min.UInt32"] = MinUInt32,
                ["Min.UInt64"] = MinUInt64,
                ["Multiply.Byte"] = MultiplyByte,
                ["Multiply.Double"] = MultiplyDouble,
                ["Multiply.Int16"] = MultiplyInt16,
                ["Multiply.Int32"] = MultiplyInt32,
                ["Multiply.Int64"] = MultiplyInt64,
                ["Multiply.SByte"] = MultiplySByte,
                ["Multiply.Single"] = MultiplySingle,
                ["Multiply.UInt16"] = MultiplyUInt16,
                ["Multiply.UInt32"] = MultiplyUInt32,
                ["Multiply.UInt64"] = MultiplyUInt64,
                ["Narrow.Double"] = NarrowDouble,
                ["Narrow.Int16"] = NarrowInt16,
                ["Narrow.Int32"] = NarrowInt32,
                ["Narrow.Int64"] = NarrowInt64,
                ["Narrow.UInt16"] = NarrowUInt16,
                ["Narrow.UInt32"] = NarrowUInt32,
                ["Narrow.UInt64"] = NarrowUInt64,
                ["Negate.Byte"] = NegateByte,
                ["Negate.Double"] = NegateDouble,
                ["Negate.Int16"] = NegateInt16,
                ["Negate.Int32"] = NegateInt32,
                ["Negate.Int64"] = NegateInt64,
                ["Negate.SByte"] = NegateSByte,
                ["Negate.Single"] = NegateSingle,
                ["Negate.UInt16"] = NegateUInt16,
                ["Negate.UInt32"] = NegateUInt32,
                ["Negate.UInt64"] = NegateUInt64,
                ["OnesComplement.Byte"] = OnesComplementByte,
                ["OnesComplement.Double"] = OnesComplementDouble,
                ["OnesComplement.Int16"] = OnesComplementInt16,
                ["OnesComplement.Int32"] = OnesComplementInt32,
                ["OnesComplement.Int64"] = OnesComplementInt64,
                ["OnesComplement.SByte"] = OnesComplementSByte,
                ["OnesComplement.Single"] = OnesComplementSingle,
                ["OnesComplement.UInt16"] = OnesComplementUInt16,
                ["OnesComplement.UInt32"] = OnesComplementUInt32,
                ["OnesComplement.UInt64"] = OnesComplementUInt64,
                ["Sqrt.Byte"] = SqrtByte,
                ["Sqrt.Double"] = SqrtDouble,
                ["Sqrt.Int16"] = SqrtInt16,
                ["Sqrt.Int32"] = SqrtInt32,
                ["Sqrt.Int64"] = SqrtInt64,
                ["Sqrt.SByte"] = SqrtSByte,
                ["Sqrt.Single"] = SqrtSingle,
                ["Sqrt.UInt16"] = SqrtUInt16,
                ["Sqrt.UInt32"] = SqrtUInt32,
                ["Sqrt.UInt64"] = SqrtUInt64,
                ["Subtract.Byte"] = SubtractByte,
                ["Subtract.Double"] = SubtractDouble,
                ["Subtract.Int16"] = SubtractInt16,
                ["Subtract.Int32"] = SubtractInt32,
                ["Subtract.Int64"] = SubtractInt64,
                ["Subtract.SByte"] = SubtractSByte,
                ["Subtract.Single"] = SubtractSingle,
                ["Subtract.UInt16"] = SubtractUInt16,
                ["Subtract.UInt32"] = SubtractUInt32,
                ["Subtract.UInt64"] = SubtractUInt64,
                ["Widen.UInt16"] = WidenUInt16,
                ["Widen.Int32"] = WidenInt32,
                ["Widen.Int64"] = WidenInt64,
                ["Widen.Int16"] = WidenInt16,
                ["Widen.Double"] = WidenDouble,
                ["Widen.UInt32"] = WidenUInt32,
                ["Widen.UInt64"] = WidenUInt64,
                ["Xor.Byte"] = XorByte,
                ["Xor.Double"] = XorDouble,
                ["Xor.Int16"] = XorInt16,
                ["Xor.Int32"] = XorInt32,
                ["Xor.Int64"] = XorInt64,
                ["Xor.SByte"] = XorSByte,
                ["Xor.Single"] = XorSingle,
                ["Xor.UInt16"] = XorUInt16,
                ["Xor.UInt32"] = XorUInt32,
                ["Xor.UInt64"] = XorUInt64,
            };
        }
    }
}
