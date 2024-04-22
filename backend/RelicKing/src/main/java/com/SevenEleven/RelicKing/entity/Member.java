package com.SevenEleven.RelicKing.entity;

import java.time.LocalDate;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;

@Entity
public class Member {

	@Id
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private int memberId;

	@Column(unique = true, length = 255)
	private String email;

	@Column(unique = true, length = 12)
	private String nickname;

	private String password;

	private int gacha;

	private int current_class_no;

	private int cumulative_lock_time;

	private int continuous_lock_date;

	private LocalDate last_lock_date;

	private boolean withdrawal_yn;

	private LocalDate created_date;

	private boolean lock_yn;
}
