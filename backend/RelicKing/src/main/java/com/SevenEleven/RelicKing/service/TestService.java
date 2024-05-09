package com.SevenEleven.RelicKing.service;

import java.util.List;
import java.util.Map;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
import com.SevenEleven.RelicKing.dto.model.MemberRelicDTO;
import com.SevenEleven.RelicKing.dto.response.InventoryResponseDTO;
import com.SevenEleven.RelicKing.dto.response.LoginResponseDTO;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.MemberRelic;
import com.SevenEleven.RelicKing.repository.MemberRelicRepository;
import com.SevenEleven.RelicKing.repository.MemberRepository;

import lombok.RequiredArgsConstructor;
import lombok.extern.log4j.Log4j2;
// Todo 삭제
@Service
@Log4j2
@RequiredArgsConstructor
@Transactional
public class TestService {

	private final MemberRepository memberRepository;

	private final MemberRelicRepository memberRelicRepository;

	private final MemberService memberService;

	@Transactional(readOnly = true)
	public LoginResponseDTO getLoginData(int memberId) {

		Member member = memberRepository.findById(memberId).orElseThrow(() -> new CustomException(ExceptionType.MEMBER_NOT_FOUND));

		Map<String, Integer> stageDifficultyInfo = memberService.getDifficulty(member);

		return LoginResponseDTO.builder()
			.memberId(member.getMemberId())
			.nickname(member.getNickname())
			.stageData(stageDifficultyInfo)
			.build();

	}

	@Transactional(readOnly = true)
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
