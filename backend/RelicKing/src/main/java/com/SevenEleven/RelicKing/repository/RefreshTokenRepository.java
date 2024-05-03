package com.SevenEleven.RelicKing.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.entity.RefreshToken;

public interface RefreshTokenRepository extends JpaRepository<RefreshToken, Integer> {

	Boolean existsByRefreshToken(String refreshToken);

	@Transactional
	void deleteByEmail(String email);
}
