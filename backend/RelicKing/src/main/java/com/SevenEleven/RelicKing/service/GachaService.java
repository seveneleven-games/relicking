package com.SevenEleven.RelicKing.service;

import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Random;
import java.util.Set;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.common.Constant;
import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
import com.SevenEleven.RelicKing.dto.request.GachaRequestDTO;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.MemberRelic;
import com.SevenEleven.RelicKing.repository.MemberRelicRepository;
import com.SevenEleven.RelicKing.repository.MemberRepository;

import lombok.RequiredArgsConstructor;
import lombok.extern.log4j.Log4j2;

@Service
@RequiredArgsConstructor
@Log4j2
@Transactional
public class GachaService {

	private final MemberRepository memberRepository;
	private final MemberRelicRepository memberRelicRepository;

	public Map<String, Integer> getGachaInfo(Member member) {
		return Map.of("gacha", member.getGacha());
	}

	public List<Map<String, Object>> doGacha(Member member, GachaRequestDTO gachaRequestDTO) {

		// 가챠권 개수 충분한지 확인
		if (gachaRequestDTO.getGachaNum().getValue() > member.getGacha()) {
			throw new CustomException(ExceptionType.NOT_ENOUGH_GACHA);
		}

		// 가챠권 차감
		member.changeGacha(member.getGacha() - gachaRequestDTO.getGachaNum().getValue());
		memberRepository.save(member);

		// 반환할 최종 결과
		List<Map<String, Object>> results = new ArrayList<>();

		int raritySize = Constant.RELIC_INFO_TABLE.length;
		int[][] gachaResult = new int[raritySize][]; // 각 유물 별 가챠로 얻은 개수를 저장할 counting array
		for (int i = 0; i < raritySize; i++) {
			int[] rarityCountingArray = new int[Constant.RELIC_INFO_TABLE[i] + 1];
			Arrays.fill(rarityCountingArray, 0);
			gachaResult[i] = rarityCountingArray;
		}

		// 가챠 돌리는 로직
		try {
			Random rand = SecureRandom.getInstanceStrong();
			for (int i = 0; i < gachaRequestDTO.getGachaNum().getValue(); i++) {
				double w = rand.nextDouble();
				for (int k = 1; k < Constant.GACHA_POSSIBILITY.length; k++) {
					if (Constant.GACHA_POSSIBILITY[k - 1] <= w && w < Constant.GACHA_POSSIBILITY[k]) {
						gachaResult[k - 1][rand.nextInt(Constant.RELIC_INFO_TABLE[k - 1]) + 1]++;
						break;
					}
				}
			}
		} catch (NoSuchAlgorithmException e) {
			throw new RuntimeException(e);
		}

		List<MemberRelic> memberRelicList = new ArrayList<>(); // 가챠 후 새로 갱신될 최종 유물 리스트
		Set<MemberRelic> memberRelics = member.getMemberRelics(); // 가챠 전 보유 중인 유물 집합
		// 기존 유물 갱신
		memberRelics.forEach(memberRelic -> {
			int relicNo = memberRelic.getRelicNo();
			int rarity = relicNo / 100;
			try {
				if (gachaResult[rarity][relicNo % 100] > 0) {
					// 경험치 더하고 레벨업 여부 따로 계산
					int before = memberRelic.getLevel();
					memberRelic.plusExp(Constant.EXP_GACHA * memberRelic.getLevel() * Constant.RARITY_WEIGHT[rarity] * gachaResult[rarity][relicNo % 100]);
					int after = memberRelic.getLevel();

					memberRelicList.add(memberRelic);

					// results에 Map add
					for (int i = 0; i < gachaResult[rarity][relicNo % 100]; i++) {
						Map<String, Object> relic = new LinkedHashMap<>();
						relic.put("relicNo", relicNo);
						relic.put("level", after);
						relic.put("levelUpYn", after > before);
						relic.put("newYn", false);
						results.add(relic);
					}

					gachaResult[rarity][memberRelic.getRelicNo() % 100] = 0;
				}
			} catch (ArrayIndexOutOfBoundsException e) {
				// db에 남아있는 과거의 유물 때문
			}
		});

		// 새로 얻은 유물 추가
		for (int rarity = 0; rarity < raritySize; rarity++) {
			for (int j = 1; j < gachaResult[rarity].length; j++) {
				if (gachaResult[rarity][j] > 0) {
					MemberRelic memberRelic = new MemberRelic(member, rarity * 100 + j);
					memberRelic.plusExp(Constant.EXP_GACHA * memberRelic.getLevel() * Constant.RARITY_WEIGHT[rarity] * (gachaResult[rarity][j] - 1));
					int after = memberRelic.getLevel();
					memberRelicList.add(memberRelic);

					for (int k = 0; k < gachaResult[rarity][j]; k++) {
						Map<String, Object> relic = new LinkedHashMap<>();
						relic.put("relicNo", memberRelic.getRelicNo());
						relic.put("level", after);
						relic.put("levelUpYn", after > 1);
						relic.put("newYn", true);
						results.add(relic);
					}
				}
			}
		}

		memberRelicRepository.saveAll(memberRelicList);
		Collections.shuffle(results);

		return results;
	}
}
