package com.SevenEleven.RelicKing.service;

import java.util.ArrayList;
import java.util.List;

import lombok.RequiredArgsConstructor;
import lombok.extern.log4j.Log4j2;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.dto.response.StageResponseDTO;
import com.SevenEleven.RelicKing.dto.response.model.RelicResponseDTO;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.MemberRelic;
import com.SevenEleven.RelicKing.repository.MemberRelicRepository;

@Service
@RequiredArgsConstructor
@Log4j2
@Transactional
public class StageService {
	public StageResponseDTO getClassAndRelics(Member member) {
		List<MemberRelic> RelicList = member.getMemberRelics().stream()
			.filter(relic -> relic.getSlot() > 0)
			.toList();

		List<RelicResponseDTO> result = new ArrayList<>(6);
		int size = RelicList.size();
		if (size < 6) {
			for (int i = size + 1; i <= 6; i++) {
				RelicResponseDTO relicResponseDTO = RelicResponseDTO.builder()
					.slot(i)
					.build();
				result.add(relicResponseDTO);
			}
		}

		return StageResponseDTO.builder()
			.relicList(result)
			.currentClassNo(member.getCurrentClassNo())
			.build();
	}

	public void patchRecord(Member member) {

	}

}
