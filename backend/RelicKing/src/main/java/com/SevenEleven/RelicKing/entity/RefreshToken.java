package com.SevenEleven.RelicKing.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;

@Entity
@Getter
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class RefreshToken {

	@Id
	@Column(name = "refresh_token_id")
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private int refreshTokenId;

	@Column(nullable = false)
	private String email;

	@Column(nullable = false)
	private String refreshToken;

	@Column(nullable = false)
	private String expiration;
}
