package com.SevenEleven.RelicKing.common.security;

import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.stereotype.Service;

import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.repository.MemberRepository;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class CustomUserDetailsService implements UserDetailsService {

	private final MemberRepository memberRepository;

	@Override
	public UserDetails loadUserByUsername(String email) throws UsernameNotFoundException {

		Member findMember = memberRepository.findByEmail(email);

		if (findMember != null) {
			return new CustomUserDetails(findMember);
		}

		throw new UsernameNotFoundException(ExceptionType.MEMBER_NOT_FOUND.getMessage());
	}
}
