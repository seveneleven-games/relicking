package com.SevenEleven.RelicKing.service;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;

import org.springframework.scheduling.annotation.Async;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.common.Constant;
import com.SevenEleven.RelicKing.dto.request.SaveLockRequestDto;
import com.SevenEleven.RelicKing.dto.response.GetLockInfoResponseDto;
import com.SevenEleven.RelicKing.dto.response.SaveLockResponseDto;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.repository.MemberRepository;

import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

@Service
@RequiredArgsConstructor
@Slf4j
public class LockService {

	private final MemberRepository memberRepository;

	@Transactional
	public SaveLockResponseDto saveLock(Member member, SaveLockRequestDto saveLockRequestDto) {

		int lockTime = saveLockRequestDto.getLockTime();

		LocalDateTime now = LocalDateTime.now();
		LocalDateTime todayMidnight = LocalDate.now().atStartOfDay();
		LocalDateTime startTime = now.minusSeconds(lockTime);

		int earnedGacha, bonusGacha, gachaAfterLock;

		// 시작 시간이 오늘인 경우
		if (startTime.isAfter(todayMidnight)) {

			// 오늘 방치 시간 업데이트
			int todayLockTimeAfterLock = member.getTodayLockTime() + lockTime;
			member.updateTodayLockTime(todayLockTimeAfterLock);

			// 연속 방치일 수 업데이트 (누적 방치 시간 업데이트 반영된 결과 바탕으로!)
			if (todayLockTimeAfterLock >= Constant.LOCK_TIME_TO_CONTINUE) {
				member.addContinuousLockDate();
			}

			// 가챠권 지급
			earnedGacha = lockTime / Constant.SECONDS_PER_GACHA;
			bonusGacha = earnedGacha * Constant.BONUS_GACHA_PERCENTAGE[Math.min(member.getContinuousLockDate(), Constant.BONUS_GACHA_PERCENTAGE.length - 1)] / 100;
			gachaAfterLock = member.getGacha() + earnedGacha + bonusGacha;
			member.changeGacha(gachaAfterLock);
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
				member.addContinuousLockDatePrev();
			}

			// 연속 방치일 수 업데이트 (누적 방치 시간 업데이트 반영된 결과 바탕으로!)
			if (todayLockTimeAfterLock >= Constant.LOCK_TIME_TO_CONTINUE) {
				member.addContinuousLockDate();
			}

			// 가챠권 지급
			earnedGacha = lockTime / Constant.SECONDS_PER_GACHA;
			int bonusGachaForYesterday = lockYesterday * Constant.BONUS_GACHA_PERCENTAGE[Math.min(member.getContinuousLockDatePrev(), Constant.BONUS_GACHA_PERCENTAGE.length - 1)] / 100;
			int bonusGachaForToday = lockToday * Constant.BONUS_GACHA_PERCENTAGE[Math.min(member.getContinuousLockDate(), Constant.BONUS_GACHA_PERCENTAGE.length - 1)] / 100;
			bonusGacha = bonusGachaForYesterday + bonusGachaForToday;
			gachaAfterLock = member.getGacha() + earnedGacha + bonusGacha;
			member.changeGacha(gachaAfterLock);
		}

		// 시작 시간이 어제든 오늘이든 전체 누적과 마지막 잠금일은 영향 받지 않으므로 조건문 타지 않음.
		int totalLockTimeAfterLock = member.getTotalLockTime() + lockTime;
		member.updateTotalLockTimeAfterLock(totalLockTimeAfterLock);
		member.updateLastLockDate();

		memberRepository.save(member);

		return SaveLockResponseDto.builder()
			.earnedGacha(earnedGacha)
			.bonusGacha(bonusGacha)
			.gachaAfterLock(gachaAfterLock)
			.build();
	}

	@Transactional(readOnly = true)
	public GetLockInfoResponseDto getLockInfo(Member member) {

		int continuousLockDate = member.getContinuousLockDate();
		int bonusGachaPercentage = Constant.BONUS_GACHA_PERCENTAGE[Math.min(continuousLockDate, Constant.BONUS_GACHA_PERCENTAGE.length - 1)];

		return GetLockInfoResponseDto.builder()
			.totalLockTime(member.getTotalLockTime())
			.continuousLockDate(continuousLockDate)
			.bonusGachaPercentage(bonusGachaPercentage)
			.todayLockTime(member.getTodayLockTime())
			.build();
	}

	@Async
	@Transactional
	@Scheduled(cron = "0 0 0 * * ?")
	public void updateLockInfo() {

		log.info("스케줄러가 실행됩니다.");

		memberRepository.updateLockInfo();

		log.info("스케줄러가 실행 완료되었습니다.");
	}
}
