package com.SevenEleven.RelicKing.service;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.Set;

import lombok.RequiredArgsConstructor;
import lombok.extern.log4j.Log4j2;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.dto.model.SkillDTO;
import com.SevenEleven.RelicKing.dto.request.StageRequestDTO;
import com.SevenEleven.RelicKing.dto.response.StageResponseDTO;
import com.SevenEleven.RelicKing.dto.model.RelicDTO;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.MemberRelic;
import com.SevenEleven.RelicKing.entity.Record;
import com.SevenEleven.RelicKing.entity.RecordRelic;
import com.SevenEleven.RelicKing.repository.MemberRelicRepository;
import com.SevenEleven.RelicKing.repository.RecordRepository;

@Service
@RequiredArgsConstructor
@Log4j2
@Transactional
public class StageService {

	private final MemberRelicRepository memberRelicRepository;
	private final RecordRepository recordRepository;

	public StageResponseDTO getClassAndRelics(Member member) {
		List<RelicDTO> relicList = new ArrayList<>();
		for (int i = 1; i <= 6; i++) {
			relicList.add(new RelicDTO(0, 0, i));
		}

		relicList.forEach(relicDTO -> {
			member.getMemberRelics().forEach(memberRelic -> {
				if (relicDTO.getSlot() == memberRelic.getSlot()) {
					relicDTO.changeRelic(memberRelic.getRelicNo(), memberRelic.getLevel(), memberRelic.getSlot());
				}
			});
		});

		relicList.sort(RelicDTO::compareTo);

		return StageResponseDTO.builder()
			.relicList(relicList)
			.currentClassNo(member.getCurrentClassNo())
			.build();
	}

	public void patchRelicAndRecord(Member member, StageRequestDTO stageRequestDTO) {
		// member relic 레벨, 경험치 갱신
		patchRelic(member,
			stageRequestDTO.getEliteKill(),
			stageRequestDTO.getNormalKill(),
			stageRequestDTO.getDifficulty());
		// 랭킹 최신화
		patchRecord(member, stageRequestDTO);
	}

	private void patchRelic(Member member, int eliteKill, int normalKill, int difficulty) {
		member.getMemberRelics().forEach(memberRelic -> {
			if (memberRelic.getSlot() > 0) {
				int earnedExp = (1000 / difficulty) + eliteKill * 10 * difficulty + normalKill * difficulty;
				memberRelic.plusExp(earnedExp);
				memberRelicRepository.save(memberRelic);
			}
		});

	}

	private void patchRecord(Member member, StageRequestDTO stageRequestDTO) {
		Optional<Record> optionalRecord = recordRepository.findByMemberAndStage(member, stageRequestDTO.getStage());
		if (optionalRecord.isEmpty()) {
			createRecord(member, stageRequestDTO);
		} else {
			Record record = optionalRecord.get();
			if (record.getDifficulty() < stageRequestDTO.getDifficulty()) {
				recordRepository.delete(record);
				createRecord(member, stageRequestDTO);
			}
		}
	}

	private void createRecord(Member member, StageRequestDTO stageRequestDTO) {

		Record record = Record.builder()
			.member(member)
			.stage(stageRequestDTO.getStage())
			.difficulty(stageRequestDTO.getDifficulty())
			.eliteKill(stageRequestDTO.getEliteKill())
			.normalKill(stageRequestDTO.getNormalKill())
			.classNo(member.getCurrentClassNo())
			.build();

		Set<MemberRelic> memberRelics = member.getMemberRelics();

		ArrayList<RecordRelic> recordRelics = new ArrayList<>(6);
		for (int i = 1; i <= 6; i++) {
			recordRelics.add(new RecordRelic(0, 0, i));
		}

		recordRelics.forEach(recordRelic -> {
			memberRelics.forEach(memberRelic -> {
				if (recordRelic.getSlot() == memberRelic.getSlot()) {
					recordRelic.changeRelic(memberRelic.getRelicNo(), memberRelic.getLevel(), memberRelic.getSlot());
				}
			});
		});

		recordRelics.sort(RecordRelic::compareTo);

		for (RecordRelic recordRelic: recordRelics) {
				record.addRecordRelic(recordRelic.getRelicNo(), recordRelic.getLevel(), recordRelic.getSlot());
			}

		log.info("========================");
		log.info(recordRelics);

		// Todo 주석 지우기
		// ArrayList<String> slotList = new ArrayList<>(List.of("1", "2", "3", "4", "5", "6"));
		// ArrayList<RecordRelic> recordRelics = new ArrayList<>(6);
		//
		// for (MemberRelic memberRelic: memberRelics) {
		// 	if (memberRelic.getSlot() > 0) {
		// 		recordRelics.add(new RecordRelic(memberRelic.getRelicNo(), memberRelic.getLevel(), memberRelic.getSlot()));
		// 		slotList.remove(Integer.toString(memberRelic.getSlot()));
		// 	}
		// }
		//
		// for (String slot: slotList) {
		// 	recordRelics.add(new RecordRelic(0, 0, Integer.parseInt(slot)));
		// }
		//
		// recordRelics.sort(RecordRelic::compareTo);
		//
		// for (RecordRelic recordRelic: recordRelics) {
		// 	record.addRecordRelic(recordRelic.getRelicNo(), recordRelic.getLevel(), recordRelic.getSlot());
		// }
		//

		for (SkillDTO skillDTO: stageRequestDTO.getSkillList()) {
			record.addRecordSkill(skillDTO.getSkillNo(), skillDTO.getLevel(), skillDTO.getSlot());
		}

		recordRepository.save(record);
	}

}
