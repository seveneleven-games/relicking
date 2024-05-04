package com.SevenEleven.RelicKing.service;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import com.SevenEleven.RelicKing.common.Constant;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.dto.model.MemberRelicDTO;
import com.SevenEleven.RelicKing.dto.request.RelicChangeRequestDTO;
import com.SevenEleven.RelicKing.dto.response.InventoryResponseDTO;
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
public class InventoryService {

	private final MemberRepository memberRepository;

	private final MemberRelicRepository memberRelicRepository;

	@Transactional(readOnly = true)
	public InventoryResponseDTO getInventoryInfo(Member member) {
		List<MemberRelic> memberRelicList = memberRelicRepository.findByMember(member);

		MemberRelicDTO[] memberRelicDTOS = new MemberRelicDTO[Constant.THE_NUMBER_OF_RELICS];
		Arrays.setAll(memberRelicDTOS, i -> new MemberRelicDTO(i + 1, 0, 0, 0));

		memberRelicList.forEach(memberRelic -> {
			memberRelicDTOS[memberRelic.getRelicNo() - 1] =
					new MemberRelicDTO(
							memberRelic.getRelicNo(),
							memberRelic.getLevel(),
							memberRelic.getExp(),
							memberRelic.getSlot());
		});

		return InventoryResponseDTO.builder()
			.currentClassNo(member.getCurrentClassNo())
			.myRelicList(new ArrayList<>(List.of(memberRelicDTOS)))
			.build();
	}

	public void changeClass(Member member, int classNo) {
		member.changeCurrentClassNo(classNo);
		memberRepository.save(member);
	}

	public void changeRelic(Member member, RelicChangeRequestDTO relicChangeRequestDTO) {
		member.getMemberRelics().forEach(memberRelic -> {
			if (memberRelic.getSlot() == relicChangeRequestDTO.getSlot()) {
				memberRelic.changeSlot(0);
				memberRelicRepository.save(memberRelic);
			}

			if (memberRelic.getRelicNo() == relicChangeRequestDTO.getRelicNo()) {
				memberRelic.changeSlot(relicChangeRequestDTO.getSlot());
				memberRelicRepository.save(memberRelic);
			}
		});
	}

}
