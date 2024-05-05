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

	public Map<String, Integer> getGachaInfo(Member member) {
		return Map.of("gacha", member.getGacha());
	}

	// Todo 유물 새로 뽑으면 db에 뽑은 순서 대로 저장 되어 유물 번호로 정렬되지 않는데, 유물 번호로 정렬되도록 회원가입하면 레벨 0 유물로 미리 채워 놓아야 할까?
	public List<Map<String, Object>> doGacha(Member member, GachaRequestDTO gachaRequestDTO) {

		if (gachaRequestDTO.getGachaNum().getValue() > member.getGacha()) {
			throw new CustomException(ExceptionType.NOT_ENOUGH_GACHA);
		}
		member.changeGacha(member.getGacha() - gachaRequestDTO.getGachaNum().getValue());
		memberRepository.save(member);

		List<Map<String, Object>> results = new ArrayList<>();

		int[] counting = new int[Constant.THE_NUMBER_OF_RELICS + 1];
		Arrays.fill(counting, 0);

		Random rand = new Random();
		for (int i = 0; i < gachaRequestDTO.getGachaNum().getValue(); i++) {
			counting[rand.nextInt(Constant.THE_NUMBER_OF_RELICS) + 1]++;
		}

		Set<MemberRelic> memberRelics = member.getMemberRelics();
		memberRelics.forEach(memberRelic -> {
			if (counting[memberRelic.getRelicNo()] > 0) {
				// 경험치 더하고 레벨업 여부 따로 계산
				int before = memberRelic.getLevel();
				memberRelic.plusExp(Constant.EXP_GACHA * counting[memberRelic.getRelicNo()]);
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
				counting[memberRelic.getRelicNo()] = 0;
			}
		});

		for (int i = 1; i <= Constant.THE_NUMBER_OF_RELICS; i++) {
			if (counting[i] > 0) {
				MemberRelic memberRelic = MemberRelic.builder()
						.member(member)
						.relicNo(i)
						.build();
				int before = memberRelic.getLevel();
				memberRelic.plusExp(Constant.EXP_GACHA * (counting[i] - 1));
				int after = memberRelic.getLevel();
				memberRelicRepository.save(memberRelic);

				Map<String, Object> relic = new LinkedHashMap<>();
				relic.put("relicNo", memberRelic.getRelicNo());
				relic.put("level", after);
				relic.put("levelUpYn", after > before);
				relic.put("newYn", true);

				results.add(relic);
			}
		}

		Collections.shuffle(results);

		return results;
	}
}
