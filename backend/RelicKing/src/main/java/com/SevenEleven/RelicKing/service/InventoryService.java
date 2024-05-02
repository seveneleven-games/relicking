package com.SevenEleven.RelicKing.service;

import java.util.List;

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

	public InventoryResponseDTO getInventoryInfo() {
		Member member = memberRepository.findByMemberId(1).orElseThrow(); // Todo 로그인한 멤버로 변경
		List<MemberRelic> memberRelicList = memberRelicRepository.findByMember(member);
		List<MemberRelicDTO> memberRelicDTOS = memberRelicList.stream().map(MemberRelic::entityToDTO).toList();

		return InventoryResponseDTO.builder()
			.currentClassNo(member.getCurrentClassNo())
			.myRelicList(memberRelicDTOS)
			.build();
	}

	public void changeClass(int classNo) {
		Member member = memberRepository.findByMemberId(1).orElseThrow(); // Todo 로그인한 멤버로 변경
		member.changeCurrentClassNo(classNo);
		memberRepository.save(member);
	}

	public void changeRelic(RelicChangeRequestDTO relicChangeRequestDTO) {
		Member member = memberRepository.findByMemberId(1).orElseThrow(); // Todo 로그인한 멤버로 변경
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
