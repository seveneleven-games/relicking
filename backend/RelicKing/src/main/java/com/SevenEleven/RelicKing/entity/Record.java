package com.SevenEleven.RelicKing.entity;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

import org.springframework.data.annotation.LastModifiedDate;
import org.springframework.data.jpa.domain.support.AuditingEntityListener;

import jakarta.persistence.CollectionTable;
import jakarta.persistence.Column;
import jakarta.persistence.ElementCollection;
import jakarta.persistence.Entity;
import jakarta.persistence.EntityListeners;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import lombok.AccessLevel;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Entity
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@ToString(exclude = {"recordRelics", "recordSkills"})
@EntityListeners(AuditingEntityListener.class)
public class Record {

	@Id
	@Column(name = "record_id")
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private int recordId;

	@ManyToOne(fetch = FetchType.LAZY)
	@JoinColumn(name = "member_id", referencedColumnName = "member_id", nullable = false)
	private Member member;

	@Column(nullable = false)
	private int stage;

	@Column(nullable = false)
	private int difficulty;

	@Column(nullable = false)
	private int eliteKill;

	@Column(nullable = false)
	private int normalKill;

	@Column(nullable = false)
	private int classNo;

	@LastModifiedDate
	@Column(nullable = false)
	@Builder.Default
	private LocalDateTime updatedDate = LocalDateTime.now();

	@ElementCollection(fetch = FetchType.LAZY)
	@CollectionTable(name = "record_relic", joinColumns = @JoinColumn(name = "record_id"))
	@Builder.Default
	private List<RecordRelic> recordRelics = new ArrayList<>(6);

	@ElementCollection(fetch = FetchType.LAZY)
	@CollectionTable(name = "record_skill", joinColumns = @JoinColumn(name = "record_id"))
	@Builder.Default
	private List<RecordSkill> recordSkills = new ArrayList<>(6);

	public void addRecordRelic(int relicNo, int level, int slot) {

		RecordRelic recordRelic = RecordRelic.builder()
			.relicNo(relicNo)
			.level(level)
			.slot(slot)
			.build();

		recordRelics.add(recordRelic);

	}

	public void addRecordSkill(int skillNo, int level, int slot) {

		RecordSkill recordSkill = RecordSkill.builder()
			.skillNo(skillNo)
			.level(level)
			.slot(slot)
			.build();

		recordSkills.add(recordSkill);

	}
}
