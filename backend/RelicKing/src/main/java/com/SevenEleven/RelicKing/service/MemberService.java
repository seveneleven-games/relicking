package com.SevenEleven.RelicKing.service;

import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;

import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
import com.SevenEleven.RelicKing.dto.request.SignUpRequestDto;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.repository.MemberRepository;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class MemberService {

	private final MemberRepository memberRepository;
	private final BCryptPasswordEncoder bCryptPasswordEncoder;

	public void signup(SignUpRequestDto dto) {

		// 이미 존재하는 이메일일 경우 예외 처리
		if (memberRepository.existsByEmail(dto.getEmail())) {
			throw new CustomException(ExceptionType.EMAIL_ALREADY_EXISTS);
		}

		Member member = dto.toEntity(bCryptPasswordEncoder.encode(dto.getPassword()));

		memberRepository.save(member);
	}
}
