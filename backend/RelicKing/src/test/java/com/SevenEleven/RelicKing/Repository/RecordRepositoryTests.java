package com.SevenEleven.RelicKing.Repository;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.Record;
import com.SevenEleven.RelicKing.repository.MemberRepository;
import com.SevenEleven.RelicKing.repository.RecordRepository;

import lombok.extern.log4j.Log4j2;

@SpringBootTest
@Log4j2
public class RecordRepositoryTests {

	@Autowired
	private RecordRepository recordRepository;

	@Autowired
	private MemberRepository memberRepository;


	@Test
	public void Test1() {
		Assertions.assertNotNull(recordRepository);
		log.info("--------------------------------------------------------------");
		log.info(recordRepository.getClass().getName());
		log.info("--------------------------------------------------------------");
	}

	@Test
	public void insertTest() {

		Member member = memberRepository.findById(1).orElseThrow();

		Record record = Record.builder()
			.member(member)
			.stage(1)
			.difficulty(1)
			.eliteKill(10)
			.normalKill(200)
			.classNo(1)
			.build();

		recordRepository.save(record);
	}

	@Test
	public void readTest() {
		Record record = recordRepository.findById(1).orElseThrow();

		log.info("--------------------------------------------------------------");
		log.info(record);
		log.info("--------------------------------------------------------------");
	}

	// @Test
	// public void updateTest() {
	// 	Record record = recordRepository.findById(1).orElseThrow();
	//
	// 	record.setStage(2);
	// 	record.setDifficulty(2);
	// 	record.setClassNo(2);
	// 	record.setEliteKill(20);
	// 	record.setNormalKill(250);
	//
	// 	recordRepository.save(record);
	//
	// }

}
