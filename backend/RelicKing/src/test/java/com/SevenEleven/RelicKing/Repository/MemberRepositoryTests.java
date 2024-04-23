package com.SevenEleven.RelicKing.Repository;

import java.util.Optional;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
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

	@Test
	public void test1() {
		Assertions.assertNotNull(memberRepository);
		log.info("--------------------------------------------------------------");
		log.info(memberRepository.getClass().getName());
		log.info("--------------------------------------------------------------");
	}

	@Test
	public void insertTest() {
		Member member = Member.builder()
			.email("test@test.com")
			.nickname("test")
			.password("1234")
			.build();

		Member result = memberRepository.save(member);

		log.info("--------------------------------------------------------------");
		log.info(result);
		log.info("--------------------------------------------------------------");
	}

	@Test
	public void readTest() {

		Optional<Member> result = memberRepository.memberWithMemberRelics(1);
		Member member = result.orElseThrow();

		log.info("--------------------------------------------------------------");
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
