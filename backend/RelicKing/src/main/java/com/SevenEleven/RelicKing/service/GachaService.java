package com.SevenEleven.RelicKing.service;

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

	// Todo 주석 지우기
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

		Random rand = new Random();

		int raritySize = Constant.RELIC_INFO_TABLE.length;
		int[][] gachaResult = new int[raritySize][];
		for (int i = 0; i < raritySize; i++) {
			int[] rarityCountingArray = new int[Constant.RELIC_INFO_TABLE[i] + 1];
			Arrays.fill(rarityCountingArray, 0);
			gachaResult[i] = rarityCountingArray;
		}

		for (int i = 0; i < gachaRequestDTO.getGachaNum().getValue(); i++) {
			double w = Math.random();
			System.out.println("===========================");
			System.out.println(w);
			if (w <= 0.6) {
				gachaResult[0][rand.nextInt(Constant.RELIC_INFO_TABLE[0]) + 1]++;
			} else if (w <= 0.85) {
				gachaResult[1][rand.nextInt(Constant.RELIC_INFO_TABLE[1]) + 1]++;
			} else if (w <= 0.95) {
				gachaResult[2][rand.nextInt(Constant.RELIC_INFO_TABLE[2]) + 1]++;
			} else if (w <= 0.99) {
				gachaResult[3][rand.nextInt(Constant.RELIC_INFO_TABLE[3]) + 1]++;
			} else {
				gachaResult[4][rand.nextInt(Constant.RELIC_INFO_TABLE[4]) + 1]++;
			}
		}

		// int[] counting = new int[Constant.THE_NUMBER_OF_C + 1];
		// Arrays.fill(counting, 0);
		//
		// for (int i = 0; i < gachaRequestDTO.getGachaNum().getValue(); i++) {
		// 	counting[rand.nextInt(Constant.THE_NUMBER_OF_C) + 1]++;
		// }
		
		Set<MemberRelic> memberRelics = member.getMemberRelics();
		memberRelics.forEach(memberRelic -> {
			int rarity = memberRelic.getRelicNo()/100;
			if (gachaResult[rarity][memberRelic.getRelicNo() % 100] > 0) {
				// 경험치 더하고 레벨업 여부 따로 계산
				int before = memberRelic.getLevel();
				memberRelic.plusExp(Constant.EXP_GACHA * gachaResult[rarity][memberRelic.getRelicNo() % 100]);
				int after = memberRelic.getLevel();

				// save
				memberRelicRepository.save(memberRelic);

				// results에 Map add
				Map<String, Object> relic = new LinkedHashMap<>();
				relic.put("relicNo", memberRelic.getRelicNo());
				relic.put("level", after);
				relic.put("levelUpYn", after > before);
				relic.put("newYn", false);

				results.add(relic);
				gachaResult[rarity][memberRelic.getRelicNo() % 100] = 0;
			}
		});

		for (int i = 0; i < raritySize; i++) {
			for (int j = 1; j < gachaResult[i].length; j++) {
				if (gachaResult[i][j] > 0) {
					MemberRelic memberRelic = MemberRelic.builder()
						.member(member)
						.relicNo(i * 100 + j)
						.build();
					memberRelic.plusExp(Constant.EXP_GACHA * (gachaResult[i][j] - 1));
					int after = memberRelic.getLevel();
					memberRelicRepository.save(memberRelic);

					Map<String, Object> relic = new LinkedHashMap<>();
					relic.put("relicNo", memberRelic.getRelicNo());
					relic.put("level", after);
					relic.put("levelUpYn", after > 1);
					relic.put("newYn", true);

					results.add(relic);
				}
			}
		}

		// for (int i = 1; i <= Constant.THE_NUMBER_OF_C; i++) {
		// 	if (counting[i] > 0) {
		// 		MemberRelic memberRelic = MemberRelic.builder()
		// 				.member(member)
		// 				.relicNo(i)
		// 				.build();
		// 		int before = memberRelic.getLevel();
		// 		memberRelic.plusExp(Constant.EXP_GACHA * (counting[i] - 1));
		// 		int after = memberRelic.getLevel();
		// 		memberRelicRepository.save(memberRelic);
		//
		// 		Map<String, Object> relic = new LinkedHashMap<>();
		// 		relic.put("relicNo", memberRelic.getRelicNo());
		// 		relic.put("level", after);
		// 		relic.put("levelUpYn", after > before);
		// 		relic.put("newYn", true);
		//
		// 		results.add(relic);
		// 	}
		// }

		Collections.shuffle(results);

		return results;
	}
}
