package com.SevenEleven.RelicKing.service;

import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.util.*;

import com.SevenEleven.RelicKing.common.Constant;
import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
import com.SevenEleven.RelicKing.dto.request.GachaRequestDTO;
import com.SevenEleven.RelicKing.entity.MemberRelic;
import com.SevenEleven.RelicKing.repository.MemberRelicRepository;
import com.SevenEleven.RelicKing.repository.MemberRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.entity.Member;

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

		if (gachaRequestDTO.getGachaNum().getValue() > member.getGacha()) {
			throw new CustomException(ExceptionType.NOT_ENOUGH_GACHA);
		}
		member.changeGacha(member.getGacha() - gachaRequestDTO.getGachaNum().getValue());
		memberRepository.save(member);

		List<Map<String, Object>> results = new ArrayList<>();

		int raritySize = Constant.RELIC_INFO_TABLE.length;
		int[][] gachaResult = new int[raritySize][];
		for (int i = 0; i < raritySize; i++) {
			int[] rarityCountingArray = new int[Constant.RELIC_INFO_TABLE[i] + 1];
			Arrays.fill(rarityCountingArray, 0);
			gachaResult[i] = rarityCountingArray;
		}

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

		List<MemberRelic> memberRelicList = new ArrayList<>();
		Set<MemberRelic> memberRelics = member.getMemberRelics();
		memberRelics.forEach(memberRelic -> {
			int rarity = memberRelic.getRelicNo()/100;
			try {
				if (gachaResult[rarity][memberRelic.getRelicNo() % 100] > 0) {
					// 경험치 더하고 레벨업 여부 따로 계산
					int before = memberRelic.getLevel();
					memberRelic.plusExp(Constant.EXP_GACHA * memberRelic.getLevel() * gachaResult[rarity][memberRelic.getRelicNo() % 100]);
					int after = memberRelic.getLevel();

					memberRelicList.add(memberRelic);

					// results에 Map add
					Map<String, Object> relic = new LinkedHashMap<>();
					relic.put("relicNo", memberRelic.getRelicNo());
					relic.put("level", after);
					relic.put("levelUpYn", after > before);
					relic.put("newYn", false);

					results.add(relic);
					gachaResult[rarity][memberRelic.getRelicNo() % 100] = 0;
				}
			} catch (ArrayIndexOutOfBoundsException e) {
				// db에 남아있는 과거의 유물 때문
			}
		});

		for (int i = 0; i < raritySize; i++) {
			for (int j = 1; j < gachaResult[i].length; j++) {
				if (gachaResult[i][j] > 0) {
					MemberRelic memberRelic = new MemberRelic(member, i * 100 + j);
					memberRelic.plusExp(Constant.EXP_GACHA * memberRelic.getLevel() * (gachaResult[i][j] - 1));
					int after = memberRelic.getLevel();
					memberRelicList.add(memberRelic);

					Map<String, Object> relic = new LinkedHashMap<>();
					relic.put("relicNo", memberRelic.getRelicNo());
					relic.put("level", after);
					relic.put("levelUpYn", after > 1);
					relic.put("newYn", true);

					results.add(relic);
				}
			}
		}

		memberRelicRepository.saveAll(memberRelicList);
		Collections.shuffle(results);

		return results;
	}
}
