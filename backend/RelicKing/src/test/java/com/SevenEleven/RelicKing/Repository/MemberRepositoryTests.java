package com.SevenEleven.RelicKing.Repository;


import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;

import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.repository.MemberRepository;

import lombok.extern.log4j.Log4j2;

@SpringBootTest
@Log4j2
public class MemberRepositoryTests {

	@Autowired
	private MemberRepository memberRepository;

	@Autowired
	private BCryptPasswordEncoder bCryptPasswordEncoder;

	@Test
	public void insertTest() {
		for (int i = 1; i <= 100 ; i++) {
			Member member = Member.builder()
					.email("test" + i + "@test.com")
					.nickname("test" + i)
					.password(bCryptPasswordEncoder.encode("abcd1234!" + i))
					.build();

			memberRepository.save(member);
		}
	}
}
