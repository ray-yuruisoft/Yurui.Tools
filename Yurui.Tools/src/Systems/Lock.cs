﻿using System;
using StackExchange.Redis;

namespace Yurui.Tools.Systems
{
    public class Lock
    {
        public Lock(RedisKey resource, RedisValue val, TimeSpan validity)
        {
            Resource = resource;
            Value = val;
            Validity = validity;
        }

        public RedisKey Resource { get; }

        public RedisValue Value { get; }

        public TimeSpan Validity { get; }
    }
}