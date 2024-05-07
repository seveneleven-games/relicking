package com.SevenEleven.RelicKing.repository;

import org.springframework.data.repository.CrudRepository;

import com.SevenEleven.RelicKing.entity.RefreshToken;

public interface RefreshTokenRepository extends CrudRepository<RefreshToken, String> {

	Boolean existsByRefreshToken(String refreshToken);
}
