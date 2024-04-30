package com.SevenEleven.RelicKing.service;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

import org.springframework.stereotype.Service;

import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
import com.SevenEleven.RelicKing.dto.model.MemberRelicDTO;
import com.SevenEleven.RelicKing.dto.response.InventoryResponseDTO;
import com.SevenEleven.RelicKing.dto.response.LoginResponseDTO;
import com.SevenEleven.RelicKing.dto.response.model.StageDifficultyDTO;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.MemberRelic;
import com.SevenEleven.RelicKing.entity.Record;
import com.SevenEleven.RelicKing.repository.MemberRelicRepository;
import com.SevenEleven.RelicKing.repository.MemberRepository;
import com.SevenEleven.RelicKing.repository.RecordRepository;

import lombok.RequiredArgsConstructor;
import lombok.extern.log4j.Log4j2;

@Service
@Log4j2
@RequiredArgsConstructor
public class TestService {

	private final MemberRepository memberRepository;

	private final MemberRelicRepository memberRelicRepository;

	private final RecordRepository recordRepository;

	public LoginResponseDTO getLoginData(int memberId) {

		Member member = memberRepository.findById(memberId).orElseThrow(() -> new CustomException(ExceptionType.MEMBER_NOT_FOUND));

		StageDifficultyDTO stageDifficultyDTO = getDifficulty(member);

		return LoginResponseDTO.builder()
			.memberId(member.getMemberId())
			.nickname(member.getNickname())
			.stageData(stageDifficultyDTO)
			.build();

	}

	public StageDifficultyDTO getDifficulty(Member member) {

		List<Integer> stage = new ArrayList<>(3);

		for (int i = 0; i <= 2; i++) {
			Optional<Record> record = recordRepository.findByMemberAndStage(member, i + 1);
			if (record.isPresent()) {
				stage.add(record.get().getDifficulty());
			} else {
				stage.add(0);
			}
		}

		return StageDifficultyDTO.builder()
			.stage1(stage.get(0))
			.stage2(stage.get(1))
			.stage3(stage.get(2))
			.build();

	}

	public InventoryResponseDTO getInventoryInfo(int memberId) {

		Member member = memberRepository.findById(memberId).orElseThrow(() -> new CustomException(ExceptionType.MEMBER_NOT_FOUND));

		List<MemberRelic> memberRelicList = memberRelicRepository.findByMember(member);

		List<MemberRelicDTO> memberRelicDTOS = memberRelicList.stream().map(MemberRelic::entityToDTO).toList();

		return InventoryResponseDTO.builder()
			.currentClassNo(member.getCurrentClassNo())
			.myRelicList(memberRelicDTOS)
			.build();

	}

}
