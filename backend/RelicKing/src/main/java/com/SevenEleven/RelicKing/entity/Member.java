package com.SevenEleven.RelicKing.entity;

import java.time.LocalDate;
import java.util.Set;

import org.springframework.data.annotation.CreatedDate;
import org.springframework.data.jpa.domain.support.AuditingEntityListener;

import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EntityListeners;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.OneToMany;
import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Entity
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor
@ToString(exclude = {"memberRelics", "records"})
@EntityListeners(AuditingEntityListener.class)
public class Member {

	@Id
	@Column(name = "member_id")
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private int memberId;

	@Column(unique = true, nullable = false)
	@Email
	@Size(min = 3, max = 255)
	private String email;

	@Column(unique = true, nullable = false)
	@Size(max = 12)
	private String nickname;

	@Column(nullable = false)
	private String password;

	@Builder.Default
	@Column(nullable = false)
	private int gacha = 0;

	@Builder.Default
	@Column(nullable = false)
	private int currentClassNo = 0;

	@Builder.Default
	@Column(nullable = false)
	private int cumulativeLockTime = 0;

	@Builder.Default
	@Column(nullable = false)
	private int continuousLockDate = 0;

	private LocalDate lastLockDate;

	@Column(nullable = false)
	private boolean withdrawalYn;

	@CreatedDate
	@Column(updatable = false, nullable = false)
	private LocalDate createdDate;

	@Column(nullable = false)
	private boolean lockYn;

	@Column(nullable = false)
	@OneToMany(mappedBy = "member", cascade = CascadeType.ALL, fetch = FetchType.LAZY)
	private Set<MemberRelic> memberRelics;

	@OneToMany(mappedBy = "member", cascade = CascadeType.ALL, fetch = FetchType.LAZY)
	private Set<Record> records;

	public void changeCurrentClassNo(int classNo) {
		this.currentClassNo = classNo;
	}

	public void changeGacha(int gacha) {
		this.gacha = gacha;
	}
}
