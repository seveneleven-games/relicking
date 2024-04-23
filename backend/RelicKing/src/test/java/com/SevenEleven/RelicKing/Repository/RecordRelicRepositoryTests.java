package com.SevenEleven.RelicKing.Repository;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

import com.SevenEleven.RelicKing.entity.Record;
import com.SevenEleven.RelicKing.entity.RecordRelic;
import com.SevenEleven.RelicKing.repository.RecordRelicRepository;
import com.SevenEleven.RelicKing.repository.RecordRepository;

import lombok.extern.log4j.Log4j2;

@SpringBootTest
@Log4j2
public class RecordRelicRepositoryTests {

	@Autowired
	private RecordRelicRepository recordRelicRepository;

	@Autowired
	private RecordRepository recordRepository;

	@Test
	public void test1() {
		Assertions.assertNotNull(recordRelicRepository);

		log.info("--------------------------------------------------------");
		log.info(recordRelicRepository.getClass().getName());
		log.info("--------------------------------------------------------");
	}

	@Test
	public void insertTest() {
		Record record = recordRepository.findById(1).orElseThrow();

		for (int i = 1; i <= 6; i++){
			RecordRelic recordRelic = RecordRelic.builder()
				.record(record)
				.relicNo(i)
				.level(10)
				.slot(i)
				.build();

			recordRelicRepository.save(recordRelic);
		}
	}

	@Test
	public void readTest() {
		RecordRelic recordRelic = recordRelicRepository.findById(1).orElseThrow();

		log.info("--------------------------------------------------------------");
		log.info(recordRelic);
		log.info("--------------------------------------------------------------");
	}
}
