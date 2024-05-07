package com.SevenEleven.RelicKing.entity;

import org.springframework.data.annotation.Id;
import org.springframework.data.redis.core.RedisHash;
import org.springframework.data.redis.core.index.Indexed;

import lombok.Builder;
import lombok.Getter;

@RedisHash(value = "RefreshToken", timeToLive = 7776000)
@Getter
@Builder
public class RefreshToken {

	@Id
	private String email;

	@Indexed
	private String refreshToken;
}
