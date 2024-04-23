package com.SevenEleven.RelicKing.entity;

import java.time.LocalDate;

import org.springframework.data.annotation.CreatedDate;
import org.springframework.data.jpa.domain.support.AuditingEntityListener;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EntityListeners;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;

@Entity
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor
@EntityListeners(AuditingEntityListener.class)
public class Member {

	@Id
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private int memberId;

	@Column(unique = true)
	@NotNull
	@Email
	@Size(min = 3, max = 255)
	private String email;

	@Column(unique = true)
	@NotNull
	@Size(max = 12)
	private String nickname;

	@NotNull
	private String password;

	@Builder.Default
	@NotNull
	private int gacha = 0;

	@Builder.Default
	@NotNull
	private int currentClassNo = 0;

	@Builder.Default
	@NotNull
	private int cumulativeLockTime = 0;

	@Builder.Default
	@NotNull
	private int continuousLockDate = 0;

	private LocalDate lastLockDate;

	@NotNull
	private boolean withdrawalYn;

	@CreatedDate
	@Column(updatable = false)
	@NotNull
	private LocalDate createdDate;

	@NotNull
	private boolean lockYn;
}
