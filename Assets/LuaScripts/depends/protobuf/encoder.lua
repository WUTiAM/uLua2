--
--------------------------------------------------------------------------------
--  FILE:  encoder.lua
--  DESCRIPTION:  protoc-gen-lua
--      Google's Protocol Buffers project, ported to lua.
--      https://code.google.com/p/protoc-gen-lua/
--
--      Copyright (c) 2010 , 林卓毅 (Zhuoyi Lin) netsnail@gmail.com
--      All rights reserved.
--
--      Use, modification and distribution are subject to the "New BSD License"
--      as listed at <url: http://www.opensource.org/licenses/bsd-license.php >.
--
--  COMPANY:  NetEase
--  CREATED:  2010年07月29日 19时30分46秒 CST
--------------------------------------------------------------------------------
--
local string = string
local table = table
local ipairs = ipairs
local assert =assert

local wire_format = require "depends/protobuf/wire_format"

local encoder = {}

function encoder._VarintSize(value)
    if value <= 0x7f then return 1 end
    if value <= 0x3fff then return 2 end
    if value <= 0x1fffff then return 3 end
    if value <= 0xfffffff then return 4 end
    return 5 
end

function encoder._SignedVarintSize(value)
    if value < 0 then return 10 end
    if value <= 0x7f then return 1 end
    if value <= 0x3fff then return 2 end
    if value <= 0x1fffff then return 3 end
    if value <= 0xfffffff then return 4 end
    return 5
end

function encoder._TagSize(field_number)
  return encoder._VarintSize(wire_format.PackTag(field_number, 0))
end

-- compute_value_size: the function calculate the value size
function encoder._SimpleSizer(compute_value_size)
    return function(field_number, is_repeated, is_packed)
        local tag_size = encoder._TagSize(field_number)
        if is_packed then
            local VarintSize = encoder._VarintSize
            return function(value)
                local result = 0
                for _, element in ipairs(value) do
                    result = result + compute_value_size(element)
                end
                return result + VarintSize(result) + tag_size
            end
        elseif is_repeated then
            return function(value)
                local result = tag_size * #value
                for _, element in ipairs(value) do
                    result = result + compute_value_size(element)
                end
                return result
            end
        else
            return function (value)
                return tag_size + compute_value_size(value)
            end
        end
    end
end

function encoder._ModifiedSizer(compute_value_size, modify_value)
    return function (field_number, is_repeated, is_packed)
        local tag_size = encoder._TagSize(field_number)
        if is_packed then
            local VarintSize = encoder._VarintSize
            return function (value)
                local result = 0
                for _, element in ipairs(value) do
                    result = result + compute_value_size(modify_value(element))
                end
                return result + VarintSize(result) + tag_size
            end
        elseif is_repeated then
            return function (value)
                local result = tag_size * #value
                for _, element in ipairs(value) do
                    result = result + compute_value_size(modify_value(element))
                end
                return result
            end
        else
            return function (value)
                return tag_size + compute_value_size(modify_value(value))
            end
        end
    end
end

function encoder._FixedSizer(value_size)
    return function (field_number, is_repeated, is_packed)
        local tag_size = encoder._TagSize(field_number)
        if is_packed then
            local VarintSize = encoder._VarintSize
            return function (value)
                local result = #value * value_size
                return result + VarintSize(result) + tag_size
            end
        elseif is_repeated then
            local element_size = value_size + tag_size
            return function(value)
                return #value * element_size
            end
        else
            local field_size = value_size + tag_size
            return function (value)
                return field_size
            end
        end
    end
end

encoder.Int32Sizer = encoder._SimpleSizer(encoder._SignedVarintSize)
encoder.Int64Sizer = encoder.Int32Sizer
encoder.EnumSizer = encoder.Int32Sizer

encoder.UInt32Sizer = encoder._SimpleSizer(encoder._VarintSize)
encoder.UInt64Sizer = encoder.UInt32Sizer 

encoder.SInt32Sizer = encoder._ModifiedSizer(encoder._SignedVarintSize, wire_format.ZigZagEncode)
encoder.SInt64Sizer = encoder.SInt32Sizer

encoder.Fixed32Sizer = encoder._FixedSizer(4) 
encoder.SFixed32Sizer = encoder.Fixed32Sizer
encoder.FloatSizer = encoder.Fixed32Sizer

encoder.Fixed64Sizer = encoder._FixedSizer(8) 
encoder.SFixed64Sizer = encoder.Fixed64Sizer
encoder.DoubleSizer = encoder.Fixed64Sizer

encoder.BoolSizer = encoder._FixedSizer(1)


function encoder.StringSizer(field_number, is_repeated, is_packed)
    local tag_size = encoder._TagSize(field_number)
    local VarintSize = encoder._VarintSize
    assert(not is_packed)
    if is_repeated then
        return function(value)
            local result = tag_size * #value
            for _, element in ipairs(value) do
                local l = #element
                result = result + VarintSize(l) + l
            end
            return result
        end
    else
        return function(value)
            local l = #value
            return tag_size + VarintSize(l) + l
        end
    end
end

function encoder.BytesSizer(field_number, is_repeated, is_packed)
    local tag_size = encoder._TagSize(field_number)
    local VarintSize = encoder._VarintSize
    assert(not is_packed)
    if is_repeated then
        return function (value)
            local result = tag_size * #value
            for _,element in ipairs(value) do
                local l = #element
                result = result + VarintSize(l) + l
            end
            return result
        end
    else
        return function (value)
            local l = #value
            return tag_size + VarintSize(l) + l
        end
    end
end

function encoder.MessageSizer(field_number, is_repeated, is_packed)
    local tag_size = encoder._TagSize(field_number)
    local VarintSize = encoder._VarintSize
    assert(not is_packed)
    if is_repeated then
        return function(value)
            local result = tag_size * #value
            for _,element in ipairs(value) do
                local l = element:ByteSize()
                result = result + VarintSize(l) + l
            end
            return result
        end
    else
        return function (value)
            local l = value:ByteSize()
            return tag_size + VarintSize(l) + l
        end
    end
end


-- ====================================================================
--  Encoders!

local _EncodeVarint = pb.varint_encoder
local _EncodeSignedVarint = pb.signed_varint_encoder


function encoder._VarintBytes(value)
    local out = {}
    local write = function(value)
        out[#out + 1 ] = value
    end
    _EncodeSignedVarint(write, value)
    return table.concat(out)
end

function encoder.TagBytes(field_number, wire_type)
  return encoder._VarintBytes(wire_format.PackTag(field_number, wire_type))
end

function encoder._SimpleEncoder(wire_type, encode_value, compute_value_size)
    return function(field_number, is_repeated, is_packed)
        if is_packed then
            local tag_bytes = encoder.TagBytes(field_number, wire_format.WIRETYPE_LENGTH_DELIMITED)
            local EncodeVarint = _EncodeVarint
            return function(write, value)
                write(tag_bytes)
                local size = 0
                for _, element in ipairs(value) do
                    size = size + compute_value_size(element)
                end
                EncodeVarint(write, size)
                for element in value do
                    encode_value(write, element)
                end
            end
        elseif is_repeated then
            local tag_bytes = encoder.TagBytes(field_number, wire_type)
            return function(write, value)
                for _, element in ipairs(value) do
                    write(tag_bytes)
                    encode_value(write, element)
                end
            end
        else
            local tag_bytes = encoder.TagBytes(field_number, wire_type)
            return function(write, value)
                write(tag_bytes)
                encode_value(write, value)
            end
        end
    end
end

function encoder._ModifiedEncoder(wire_type, encode_value, compute_value_size, modify_value)
    return function (field_number, is_repeated, is_packed)
        if is_packed then
            local tag_bytes = encoder.TagBytes(field_number, wire_format.WIRETYPE_LENGTH_DELIMITED)
            local EncodeVarint = _EncodeVarint
            return function (write, value)
                write(tag_bytes)
                local size = 0
                for _, element in ipairs(value) do
                    size = size + compute_value_size(modify_value(element))
                end
                EncodeVarint(write, size)
                for _, element in ipairs(value) do
                    encode_value(write, modify_value(element))
                end
            end
        elseif is_repeated then
            local tag_bytes = encoder.TagBytes(field_number, wire_type)
            return function (write, value)
                for _, element in ipairs(value) do
                    write(tag_bytes)
                    encode_value(write, modify_value(element))
                end
            end
        else
            local tag_bytes = encoder.TagBytes(field_number, wire_type)
            return function (write, value)
                write(tag_bytes)
                encode_value(write, modify_value(value))
            end
        end
    end
end

function encoder._StructPackEncoder(wire_type, value_size, format)
    return function(field_number, is_repeated, is_packed)
        local struct_pack = pb.struct_pack
        if is_packed then
            local tag_bytes = encoder.TagBytes(field_number, wire_format.WIRETYPE_LENGTH_DELIMITED)
            local EncodeVarint = _EncodeVarint
            return function (write, value)
                write(tag_bytes)
                EncodeVarint(write, #value * value_size)
                for _, element in ipairs(value) do
                    struct_pack(write, format, element)
                end
            end
        elseif is_repeated then
            local tag_bytes = encoder.TagBytes(field_number, wire_type)
            return function (write, value)
                for _, element in ipairs(value) do
                    write(tag_bytes)
                    struct_pack(write, format, element)
                end
            end
        else
            local tag_bytes = encoder.TagBytes(field_number, wire_type)
            return function (write, value)
                write(tag_bytes)
                struct_pack(write, format, value)
            end
        end

    end
end

encoder.Int32Encoder = encoder._SimpleEncoder(wire_format.WIRETYPE_VARINT, _EncodeSignedVarint, encoder._SignedVarintSize)
encoder.Int64Encoder = encoder.Int32Encoder
encoder.EnumEncoder = encoder.Int32Encoder

encoder.UInt32Encoder = encoder._SimpleEncoder(wire_format.WIRETYPE_VARINT, _EncodeVarint, encoder._VarintSize)
encoder.UInt64Encoder = encoder.UInt32Encoder

encoder.SInt32Encoder = encoder._ModifiedEncoder(
    wire_format.WIRETYPE_VARINT, _EncodeVarint, encoder._VarintSize,
    wire_format.ZigZagEncode32)

encoder.SInt64Encoder = encoder._ModifiedEncoder(
    wire_format.WIRETYPE_VARINT, _EncodeVarint, encoder._VarintSize,
    wire_format.ZigZagEncode64)

encoder.Fixed32Encoder  = encoder._StructPackEncoder(wire_format.WIRETYPE_FIXED32, 4, string.byte('I'))
encoder.Fixed64Encoder  = encoder._StructPackEncoder(wire_format.WIRETYPE_FIXED64, 8, string.byte('Q'))
encoder.SFixed32Encoder = encoder._StructPackEncoder(wire_format.WIRETYPE_FIXED32, 4, string.byte('i'))
encoder.SFixed64Encoder = encoder._StructPackEncoder(wire_format.WIRETYPE_FIXED64, 8, string.byte('q'))
encoder.FloatEncoder    = encoder._StructPackEncoder(wire_format.WIRETYPE_FIXED32, 4, string.byte('f'))
encoder.DoubleEncoder   = encoder._StructPackEncoder(wire_format.WIRETYPE_FIXED64, 8, string.byte('d'))


function encoder.BoolEncoder(field_number, is_repeated, is_packed)
    local false_byte = '\0'
    local true_byte = '\1'
    if is_packed then
        local tag_bytes = encoder.TagBytes(field_number, wire_format.WIRETYPE_LENGTH_DELIMITED)
        local EncodeVarint = _EncodeVarint
        return function (write, value)
            write(tag_bytes)
            EncodeVarint(write, #value)
            for _, element in ipairs(value) do
                if element then
                    write(true_byte)
                else
                    write(false_byte)
                end
            end
        end
    elseif is_repeated then
        local tag_bytes = encoder.TagBytes(field_number, wire_format.WIRETYPE_VARINT)
        return function(write, value)
            for _, element in ipairs(value) do
                write(tag_bytes)
                if element then
                    write(true_byte)
                else
                    write(false_byte)
                end
            end
        end
    else
        local tag_bytes = encoder.TagBytes(field_number, wire_format.WIRETYPE_VARINT)
        return function (write, value)
            write(tag_bytes)
            if value then
                return write(true_byte)
            end
            return write(false_byte)
        end
    end
end

function encoder.StringEncoder(field_number, is_repeated, is_packed)
    local tag = encoder.TagBytes(field_number, wire_format.WIRETYPE_LENGTH_DELIMITED)
    local EncodeVarint = _EncodeVarint
    assert(not is_packed)
    if is_repeated then
        return function (write, value)
            for _, element in ipairs(value) do
--                encoded = element.encode('utf-8')
                write(tag)
                EncodeVarint(write, #element)
                write(element)
            end
        end
    else
        return function (write, value)
--            local encoded = value.encode('utf-8')
            write(tag)
            EncodeVarint(write, #value)
            return write(value)
        end
    end
end

function encoder.BytesEncoder(field_number, is_repeated, is_packed)
    local tag = encoder.TagBytes(field_number, wire_format.WIRETYPE_LENGTH_DELIMITED)
    local EncodeVarint = _EncodeVarint
    assert(not is_packed)
    if is_repeated then
        return function (write, value)
            for _, element in ipairs(value) do
                write(tag)
                EncodeVarint(write, #element)
                write(element)
            end
        end
    else
        return function(write, value)
            write(tag)
            EncodeVarint(write, #value)
            return write(value)
        end
    end
end


function encoder.MessageEncoder(field_number, is_repeated, is_packed)
    local tag = encoder.TagBytes(field_number, wire_format.WIRETYPE_LENGTH_DELIMITED)
    local EncodeVarint = _EncodeVarint
    assert(not is_packed)
    if is_repeated then
        return function(write, value)
            for _, element in ipairs(value) do
                write(tag)
                EncodeVarint(write, element:ByteSize())
                element:_InternalSerialize(write)
            end
        end
    else
        return function (write, value)
            write(tag)
            EncodeVarint(write, value:ByteSize())
            return value:_InternalSerialize(write)
        end
    end
end

return encoder
