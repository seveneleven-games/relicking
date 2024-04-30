package com.SevenEleven.RelicKing.service;

import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.repository.MemberRepository;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class MemberService {

	private final MemberRepository memberRepository;

	public Response checkEmailForDuplicates(String email) {
		boolean isDuplicate = memberRepository.existsByEmail(email);

		if (isDuplicate) {
			return new Response(HttpStatus.CONFLICT.value(), "사용 불가능한 이메일입니다.", false);
		} else {
			return new Response(HttpStatus.OK.value(), "사용 가능한 이메일입니다.", true);
		}
	}

	public Response checkNicknameForDuplicates(String nickname) {
		boolean isDuplicate = memberRepository.existsByNickname(nickname);

		if (isDuplicate) {
			return new Response(HttpStatus.CONFLICT.value(), "사용 불가능한 닉네임 입니다.", false);
		} else {
			return new Response(HttpStatus.OK.value(), "사용 가능한 닉네임 입니다.", true);
		}
	}

}
