package com.SevenEleven.RelicKing.Repository;

import java.util.Optional;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.test.annotation.Commit;
import org.springframework.transaction.annotation.Transactional;

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
	public void test1() {
		Assertions.assertNotNull(memberRepository);
		log.info("--------------------------------------------------------------");
		log.info(memberRepository.getClass().getName());
		log.info("--------------------------------------------------------------");
	}

	@Test
	public void insertTest() {
		for (int i = 1; i <= 100 ; i++) {
			Member member = Member.builder()
					.email("test" + i + "@test.com")
					.nickname("test" + i)
					.password(bCryptPasswordEncoder.encode("abcd1234!" + i))
					.gacha(1000)
					.build();

			memberRepository.save(member);
		}
	}

	@Test
	public void readTest() {

		Optional<Member> result = memberRepository.findByMemberId(1);
		Member member = result.orElseThrow();

		log.info("--------------------------------------------------------------");
		log.info(member);
		log.info(member.getRecords());
		log.info(member.getMemberRelics());
		log.info("--------------------------------------------------------------");

	}

	@Test
	@Commit
	@Transactional
	public void deleteTest() {
		memberRepository.deleteById(9);
	}
}
