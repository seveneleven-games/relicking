package com.SevenEleven.RelicKing.service;

import java.util.List;
import java.util.Set;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
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
		List<MemberRelic> memberRelicList = new java.util.ArrayList<>(member.getMemberRelics().stream().toList());

		memberRelicList.sort(MemberRelic::compareTo);

		return InventoryResponseDTO.builder()
			.currentClassNo(member.getCurrentClassNo())
			.myRelicList(memberRelicList.stream().map(MemberRelic::entityToDTO).toList())
			.build();
	}

	public void changeClass(Member member, int classNo) {
		member.changeCurrentClassNo(classNo);
		memberRepository.save(member);
	}

	public void changeRelic(Member member, RelicChangeRequestDTO relicChangeRequestDTO) {
		int requestRelicNo = relicChangeRequestDTO.getRelicNo();
		int requestSlotNo = relicChangeRequestDTO.getSlot();
		Set<MemberRelic> memberRelics = member.getMemberRelics();

		if (requestRelicNo != 0 &&
			memberRelics.stream().
				noneMatch(relic -> relic.getRelicNo() == requestRelicNo)) {
			throw new CustomException(ExceptionType.NO_SUCH_RELIC);
		}

		memberRelics.forEach(memberRelic -> {
			if (memberRelic.getSlot() == requestSlotNo) {
				memberRelic.changeSlot(0);
				memberRelicRepository.save(memberRelic);
			}

			if (memberRelic.getRelicNo() == requestRelicNo) {
				memberRelic.changeSlot(requestSlotNo);
				memberRelicRepository.save(memberRelic);
			}
		});
	}

}
