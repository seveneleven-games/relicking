package com.SevenEleven.RelicKing.entity;

import java.time.LocalDate;
import java.util.LinkedHashSet;
import java.util.Set;

import org.springframework.data.annotation.CreatedDate;
import org.springframework.data.jpa.domain.support.AuditingEntityListener;

import com.SevenEleven.RelicKing.common.Constant;

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
import jakarta.validation.constraints.Size;
import lombok.AccessLevel;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Entity
@Getter
@NoArgsConstructor(access = AccessLevel.PROTECTED)
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

	@Column(nullable = false)
	private int gacha = Constant.INITIAL_GACHA;

	@Column(nullable = false)
	private int currentClassNo = 1;

	@Column(nullable = false)
	private int todayLockTime = 0;

	@Column(nullable = false)
	private int yesterdayLockTime = 0;

	@Column(nullable = false)
	private int totalLockTime = 0;

	@Column(nullable = false)
	private int continuousLockDate = 0;

	@Column(nullable = false)
	private int continuousLockDatePrev = 0;

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
	private final Set<MemberRelic> memberRelics = new LinkedHashSet<>();

	@OneToMany(mappedBy = "member", cascade = CascadeType.ALL, fetch = FetchType.LAZY)
	private final Set<Record> records = new LinkedHashSet<>();

	@Builder
	public Member(String email, String nickname, String password) {
		this.email = email;
		this.nickname = nickname;
		this.password = password;
	}

	public void changeCurrentClassNo(int classNo) {
		this.currentClassNo = classNo;
	}

	public void changeGacha(int gacha) {
		this.gacha = gacha;
	}

	public void updateTodayLockTime(int todayLockTimeAfterLock) {
		this.todayLockTime = todayLockTimeAfterLock;
	}

	public void updateYesterdayLockTime(int yesterdayLockTimeAfterLock) {
		this.yesterdayLockTime = yesterdayLockTimeAfterLock;
	}

	public void updateTotalLockTimeAfterLock(int totalLockTimeAfterLock) {
		this.totalLockTime = totalLockTimeAfterLock;
	}

	public void updateLastLockDate() {
		this.lastLockDate = LocalDate.now();
	}

	public void addContinuousLockDate() {
		this.continuousLockDate = this.continuousLockDatePrev + 1;
	}

	public void addContinuousLockDatePrev() {
		this.continuousLockDatePrev++;
	}

	public void updateNickname(String newNickname) {
		this.nickname = newNickname;
	}

	public void updatePassword(String newEncryptedPassword) {
		this.password = newEncryptedPassword;
	}
}
