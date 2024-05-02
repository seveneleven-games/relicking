package com.SevenEleven.RelicKing.service;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.common.Constant;
import com.SevenEleven.RelicKing.dto.request.SaveLockRequestDto;
import com.SevenEleven.RelicKing.dto.response.GetLockInfoResponseDto;
import com.SevenEleven.RelicKing.dto.response.SaveLockResponseDto;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.repository.MemberRepository;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class LockService {

	private final MemberRepository memberRepository;

	@Transactional
	public SaveLockResponseDto saveLock(Member member, SaveLockRequestDto saveLockRequestDto) {

		int lockTime = saveLockRequestDto.getLockTime();

		LocalDateTime now = LocalDateTime.now();
		LocalDateTime todayMidnight = LocalDate.now().atStartOfDay();
		LocalDateTime startTime = now.minusSeconds(lockTime);

		// 시작 시간이 오늘인 경우
		if (startTime.isAfter(todayMidnight)) {

			// 오늘 방치 시간 업데이트
			int todayLockTimeAfterLock = member.getTodayLockTime() + lockTime;
			member.updateTodayLockTime(todayLockTimeAfterLock);

			// 연속 방치일 수 업데이트 (누적 방치 시간 업데이트 반영된 결과 바탕으로!)
			if (todayLockTimeAfterLock >= Constant.LOCK_TIME_TO_CONTINUE) {
				member.addContinuousLockDate();
			}

		}
		// 시작 시간이 어제인 경우
		else {

			// 방치 시간 중 어제 분량, 오늘 분량
			int lockToday = (int)ChronoUnit.SECONDS.between(todayMidnight, now);
			int lockYesterday = lockTime - lockToday;

			// 어제 방치 시간 업데이트
			int yesterdayLockTimeAfterLock = member.getYesterdayLockTime() + lockYesterday;
			member.updateYesterdayLockTime(yesterdayLockTimeAfterLock);

			// 오늘 방치 시간 업데이트
			int todayLockTimeAfterLock = member.getTodayLockTime() + lockToday;
			member.updateTodayLockTime(todayLockTimeAfterLock);

			// 어제 시간에 어제 분량 더하면 스트릭이 유지되는 경우
			if (yesterdayLockTimeAfterLock >= Constant.LOCK_TIME_TO_CONTINUE) {
				member.addContinuousLockDate();
			}

		}

		// 시작 시간이 어제든 오늘이든 전체 누적과 마지막 잠금일은 영향 받지 않으므로 조건문 타지 않음.
		int totalLockTimeAfterLock = member.getTotalLockTime() + lockTime;
		member.updateTotalLockTimeAfterLock(totalLockTimeAfterLock);
		member.updateLastLockDate();

		// 가챠권 지급
		int earnedGacha = lockTime / Constant.GACHA_PER_MINUTE;
		int gachaAfterLock = member.getGacha() + earnedGacha;
		member.changeGacha(gachaAfterLock);

		memberRepository.save(member);

		return SaveLockResponseDto.builder()
			.earnedGacha(earnedGacha)
			.gachaAfterLock(gachaAfterLock)
			.build();
	}

	@Transactional(readOnly = true)
	public GetLockInfoResponseDto getLockInfo(Member member) {
		return GetLockInfoResponseDto.builder()
			.totalLockTime(member.getTotalLockTime())
			.continuousLockDate(member.getContinuousLockDate())
			.todayLockTime(member.getTodayLockTime())
			.build();
	}
}
